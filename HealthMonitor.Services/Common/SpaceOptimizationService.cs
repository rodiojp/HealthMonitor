using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Domain.SpaceOptimization;
using HealthMonitor.Services.Interfaces;
using HealthMonitor.Domain;
namespace HealthMonitor.Services.Common
{
    public class SpaceOptimizationService : ISpaceOptimizationService
    {
        private Threshold<DateTime> fileHoldLimit;
        private Threshold<long> fileSizeLimit;

        public SpaceOptimizationService(int maxDays, long maxSize)
        {
            //fileHoldLimit = new Treshold<DateTime>(DateTime.Now.AddDays(-maxDays), DateTime.Now);
            MaxDays = maxDays;
            //fileSizeLimit = new Treshold<long>(0, maxSize);
            MaxSize = maxSize;
        }
        /// <summary>
        ///  Default Constructor
        /// </summary>
        public SpaceOptimizationService()
        { 
        }


        /// <summary>
        /// Set the maximum days before file needs to be deleted
        /// </summary>
        public int MaxDays { set => fileHoldLimit = new Threshold<DateTime>(DateTime.Now.AddDays(-value), DateTime.Now); }
        /// <summary>
        /// Set the maximum size before the file needs to be archived
        /// </summary>
        public long MaxSize { set => fileSizeLimit = new Threshold<long>(0, value); }
        /// <summary>
        /// Determin what action, if any, needs to be done for space optimization
        /// </summary>
        /// <param name="fileInfo">a <see cref="FileInfo"/> object</param>
        /// <returns>a <see cref="SpaceSavedAction"/> object</returns>
        public SpaceSavedAction ActionRequired(FileInfo fileInfo)
        {
            return ActionRequired(fileInfo.Length, fileInfo.LastWriteTime);
        }
        /// <summary>
        /// Determin what action, if any, needs to be done for space optimization
        /// </summary>
        /// <param name="fileSize">The size of the file in bytes</param>
        /// <param name="fileCreatedDate">The file date time</param>
        /// <returns></returns>
        public SpaceSavedAction ActionRequired(long fileSize, DateTime fileCreatedDate)
        {
            var result = SpaceSavedAction.None;
            if (fileHoldLimit.ExceedsBounds(fileCreatedDate))
                result = SpaceSavedAction.Deleted;
            else if (fileSizeLimit.ExceedsBounds(fileSize))
                result = SpaceSavedAction.Archived;
            return result;
        }
    }
}
