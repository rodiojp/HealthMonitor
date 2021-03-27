using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Domain.SpaceOptimization.Dtos;
using HealthMonitor.WindowsServices.Interfaces;

namespace HealthMonitor.Services.Common
{
    public class ArchivingService : IArchiveService
    {
        private string archiveFileName;
        public string ArchiveFileName
        {
            set
            {
                archiveFileName = value;
                //if file does not exists create one
                if (!File.Exists(value))
                    BuildZipFile(value);
            }
        }
        /// <summary>
        /// Constructor to Service
        /// </summary>
        /// <param name="archiveFile">The archive file will acting upon</param>
        public ArchivingService(string archiveFile)
        {
            if (string.IsNullOrEmpty(archiveFile))
                throw new ArgumentNullException(nameof(archiveFile));
            //fullPath = archiveFile;
            //if (!File.Exists(archiveFile))
            //    BuildZipFile(archiveFile);
            ArchiveFileName = archiveFile;
        }

        public ArchivingService()
        {
        }
        /// <summary>
        /// Create a zip file to store big files in for the better compression
        /// </summary>
        /// <param name="archiveFile"></param>
        private void BuildZipFile(string archiveFile)
        {
            using (var archive = ZipFile.Open(archiveFile, ZipArchiveMode.Create))
            {  }
        }
        /// <summary>
        /// Add a single file
        /// </summary>
        /// <param name="file">Key Value pair of the name of the file and its
        /// DateTime to be set. Setting of its DateTime is important to know in case
        /// it is over x days</param>
        public void AddFile(KeyValuePair<string, DateTime> file)
        {
            using (var zipStream = new FileStream(archiveFileName, FileMode.Open))
            {
                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Update))
                {
                    CreateEntry(archive, file);
                }
            }
        }
        /// <summary>
        /// Stream the file to the zip
        /// </summary>
        /// <param name="archive">A <see cref="ZipArchive"/> instantiated object</param>
        /// <param name="file"><see cref="KeyValuePair<string, DateTime>"/> of fileName and Date time to set entry for</param>
        private void CreateEntry(ZipArchive archive, KeyValuePair<string, DateTime> file)
        {
            if (string.IsNullOrEmpty(Path.GetFileName(file.Key)))
                throw new ArgumentException($"{file.Key} must return a file name");
            var entry = archive.CreateEntry(Path.GetFileName(file.Key));
            entry.LastWriteTime = file.Value;
            using (var zipFile = entry.Open())
            {
                var data = File.ReadAllBytes(file.Key);
                zipFile.Write(data, 0, data.Length);
            }
        }
        /// <summary>
        /// Add a bunch of files that need to be archived / zipped in ab existig zip file
        /// </summary>
        /// <param name="files">
        /// A Directory of files with thir name
        /// and DateTime Values. Use the DateTime value to track
        /// days old.
        /// </param>
        public void AddFiles(IDictionary<string, DateTime> files)
        {
            using (var zipStream = new FileStream(archiveFileName, FileMode.Open))
            {
                using(var archive = new ZipArchive(zipStream, ZipArchiveMode.Update))
                {
                    foreach (var file in files)
                    {
                        CreateEntry(archive, file);
                    }
                }
            }
        }
        /// <summary>
        /// Deletes the file from the archive file <see cref="ArchiveFileName"/>
        /// </summary>
        /// <param name="fileName">the file name. Not the full path</param>
        public void DeleteFile(string fileName)
        {
            using (var zipStream = new FileStream(archiveFileName, FileMode.Open))
            {
                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Update))
                {
                    archive.GetEntry(fileName).Delete();
                }
            }
        }
        /// <summary>
        /// Get the contents of the archive file
        /// </summary>
        /// <returns>A list of <see cref="ArchiveDto"/> objects</returns>
        public IEnumerable<ArchiveDto> GetArchiveFiles()
        {
            var results = new List<ArchiveDto>();
            using (var archive = ZipFile.OpenRead(archiveFileName))
            {
                results.AddRange(archive.Entries.Select(entry => new ArchiveDto(entry.Name, entry.LastWriteTime.Date, entry.Length)));
            }
            return results;
        }
    }
}
