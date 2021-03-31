using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Domain.SpaceOptimization;
using HealthMonitor.Services.Interfaces;
using HealthMonitor.WindowsServices.Interfaces;
using HealthMonitor.Domain.Results;
using HealthMonitor.Domain.Extensions;
using log4net;
using HealthMonitor.Domain;
using System.ServiceProcess;

using System.IO;
using HealthMonitor.Domain.SpaceOptimization.Dtos;

namespace HealthMonitor.Application.DoHealthChecks
{
    /// <summary>
    /// Space Optimization - currently will delete any file that was lastmodified over x days
    /// and archive anything over x bytes
    /// </summary>
    /// <remarks>
    /// Class: SpaceOpimizationCheck.cs
    /// Note: This could be refactored and improved upon. Have the ArchiveDto and FileInfo
    /// new classes reuquired, implenent some interface and instead of having two methods,
    /// one from the files and one for the archive, do both.
    /// </remarks>
    public class SpaceOpimizationCheck : StopStartWindowsServices
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const string ZIP_FILE_NAME = "SpaceSaved.zip";
        private readonly SpaceSavedAction[] takeActionItems = new[] { SpaceSavedAction.Deleted, SpaceSavedAction.Archived };
        private readonly IArchiveService archiveService;
        private readonly ISpaceOptimizationService spaceOptimizationService;
        /// <summary>
        /// Default Constructor
        /// </summary>
        public SpaceOpimizationCheck()
        {
            Name = "Space Opimization";
            HealthType = HealthType.SpaceOptimization;

        }

        public SpaceOpimizationCheck(IArchiveService archiveService, ISpaceOptimizationService spaceSavingsService) : this()
        {
            this.archiveService = archiveService;
            spaceOptimizationService = spaceSavingsService;
        }


        /// <summary>
        /// Iterate through all prescribed directories and look to optimeze by either deleting or
        /// archiveing/zipping files
        /// </summary>
        /// <param name="healthCheckParameters">parameters that include size of file before archiving and directories to work on</param>
        /// <returns> object</returns>
        public override HealthMonitorResult DoHealthCheck(IEnumerable<BoundsLimit> healthCheckParameters)
        {
            const string maxDays = "maxDays";
            const string maxSize = "maxSize";

            HealthMonitorResult result = new HealthMonitorResult(Name, HealthType, ResultStatus.Information);

            string[] spaceOptimizeCheckParams = new[] { maxDays, maxSize };
            BoundsLimit[] checkParameters = healthCheckParameters as BoundsLimit[] ?? healthCheckParameters.ToArray();
            EnsureAllParametersArePresent(spaceOptimizeCheckParams, checkParameters);
            int daysOld = int.Parse(checkParameters.Single(x => x.Name.Equals(maxDays)).Value);
            long fileSize = long.Parse(checkParameters.Single(x => x.Name.Equals(maxSize)).Value);
            spaceOptimizationService.MaxDays = daysOld;
            spaceOptimizationService.MaxSize = fileSize;
            BoundsLimit boundsLimit = checkParameters.FirstOrDefault(x => x.Name.ToLower().Equals(ServiceTimeoutMillisecondsParameter.ToLower()));
            if (boundsLimit != null)
            {
                int serviceTimeoutMilliseconds = int.Parse(boundsLimit.Value);
                ServiceTimeout = TimeSpan.FromMilliseconds(serviceTimeoutMilliseconds);
            }
            //the list of folder(s) to work on are of type 'folder'
            List<string> directories = checkParameters.Where(x => x.Type.Equals("folder"))
                                                                     .Select(y => y.Value)
                                                                     .ToList();
            ServiceController[] services = checkParameters.Where(x => x.Type.Equals("service"))
                                                                   .Select(y => new ServiceController(y.Value))
                                                                   .ToArray();
            HealthMonitorResult resultToStop = StopServices(services);
            result.MessageBuilder.AppendNewLine(resultToStop.MessageBuilder.ToString());
            List<SpaceOptimizationSummary> optimizations = new List<SpaceOptimizationSummary>();
            //iterete through directories
            foreach (string path in directories)
            {
                try
                {
                    DirectoryInfo directory = new DirectoryInfo(path);
                    archiveService.ArchiveFileName = Path.Combine(path, ZIP_FILE_NAME);
                    SpaceOptimizationSummary summary = new SpaceOptimizationSummary(DateTime.Now,
                                                                                    new List<FileOptimized>(),
                                                                                    path,
                                                                                    directory.GetCurrentSizeExceptFiles(".zip"));
                    result.MessageBuilder.AppendNewLine(DeleteOldArchivedFiles(summary, daysOld));
                    result.MessageBuilder.AppendNewLine(DeleteOldArchivedFiles(summary, path));
                    summary.CurrentSize = directory.GetCurrentSizeExceptFiles(".zip");
                    result.MessageBuilder.AppendNewLine(summary.ToString());
                    optimizations.Add(summary);
                }
                catch (Exception e)
                {
                    result.MessageBuilder.AppendNewLine(e.ToLogString());
                    result.Status = ResultStatus.Warning;
                }
            }
            HealthMonitorResult resultToStart = StartServices(services);
            result.MessageBuilder.AppendNewLine(resultToStart.MessageBuilder.ToString());
            string spaceSavedMessage = string.Format(SystemConstants.TOTAL_FILE_SIZE_MSG,
                                                     FileSizeUtils.FileSizeDescription(optimizations.Sum(x => x.SpaceSaved)));
            result.MessageBuilder.AppendNewLine(spaceSavedMessage);
            return result;
        }
        /// <summary>
        /// Deletes or adds to the SpaceSaved.zip files
        /// </summary>
        /// <param name="summary">A <see cref="SpaceOptimizationSummary"/> object of record transaction for directory</param>
        /// <param name="path">Actual directory</param>
        /// <returns>A string of any failed files that need to be archived of deleted</returns>
        private string DeleteOldArchivedFiles(SpaceOptimizationSummary summary, string path)
        {
            StringBuilder sb = new StringBuilder();
            //go through all the files and try to delete them from zip one
            //at a time
            foreach (FileInfo item in GetAllFilesThatNeedToBeArchivedOrDeleted(path))
            {
                try
                {
                    summary.FilesOptimized.Add(DoSpaceSavingAction(item));
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                    sb.AppendNewLine(e.Message);
                }
            }
            return sb.ToString();
        }

        private FileOptimized DoSpaceSavingAction(FileInfo fileInfo)
        {
            FileOptimized result = new FileOptimized(fileInfo);
            //Delete it if it has not been accessed for x days
            if (spaceOptimizationService.ActionRequired(fileInfo) == SpaceSavedAction.Deleted)
            {
                result.SavedAction = SpaceSavedAction.Deleted;
            }
            //We know that if it was deleted it needs to be archived
            else
            {
                archiveService.AddFile(new KeyValuePair<string, DateTime>(fileInfo.FullName, fileInfo.CreationTime));
                result.SavedAction = SpaceSavedAction.Archived;
            }
            // doesn't matter if it was archived or needs deleting 
            // if archived we have copy in zip , if deleted,
            // we're going to delete anyway
            File.Delete(fileInfo.FullName);
            return result;
        }

        /// <summary>
        /// Get all the files that need to be either deleted or archived to save space for directory
        /// </summary>
        /// <param name="path">Actual directory</param>
        /// <returns></returns>
        private IEnumerable<FileInfo> GetAllFilesThatNeedToBeArchivedOrDeleted(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            return directory.GetFilesExceptFiles(".zip")
                .Where(item => takeActionItems.Contains(spaceOptimizationService.ActionRequired(item)))
                .OrderBy(x => x.CreationTime);
        }


        /// <summary>
        /// Deletes or adds to the SpaceSaved.zip file
        /// </summary>
        /// <param name="summary">A <see cref="SpaceOptimizationSummary"/> object of record transaction for directory</param>
        /// <param name="daysOld">Number of days before deleting from zip</param>
        /// <returns>A string of any failed files that need to be archived of deleted</returns>
        private string DeleteOldArchivedFiles(SpaceOptimizationSummary summary, int daysOld)
        {
            StringBuilder sb = new StringBuilder();
            //go through all the files and try to delete them from zip one
            //at a time
            foreach (ArchiveDto item in GetAllArchivedFilesThatAreTooOld(daysOld))
            {
                try
                {
                    summary.FilesOptimized.Add(DoSpaceSavingAction(item));
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                    sb.AppendNewLine(e.Message);
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// Search through the zip file for files older than daysOld
        /// </summary>
        /// <param name="daysOld">Number of days old before deleting from zip</param>
        /// <returns>An array of <see cref="ArchiveDto"/> objects</returns>
        private IEnumerable<ArchiveDto> GetAllArchivedFilesThatAreTooOld(int daysOld)
        {
            return archiveService.GetArchiveFiles()
                .Where(x => x.CreatedDate < DateTime.Now.AddDays(-daysOld))
                .OrderBy(x => x.CreatedDate);
        }

        /// <summary>
        /// Delete the specified file in the zip file
        /// </summary>
        /// <param name="dto">A <see cref="ArchiveDto"/> object</param>
        /// <returns>A <see cref="FileOptimized"/> object indication it was deleted</returns>
        private FileOptimized DoSpaceSavingAction(ArchiveDto dto)
        {
            archiveService.DeleteFile(dto.FullName);
            return new FileOptimized(dto.FullName, dto.Size, dto.FullName, SpaceSavedAction.Deleted);
        }
    }
}