using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitor.Domain.SpaceOptimization
{
    /// <summary>
    /// Represents a file that needed to be optimized for space saving
    /// </summary>
    /// <remarks>
    /// Class: FileOptimized
    /// </remarks>
    public class FileOptimized
    {
        /// <summary>
        /// The directory that the file was optimized
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// The file name
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// The size in bytes
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// What action was taken to optimize
        /// </summary>
        public SpaceSavedAction SavedAction { get; set; }

        public FileOptimized(FileInfo fileInfo)
        {
            FullPath = fileInfo.DirectoryName;
            FileName = fileInfo.FullName;
            FileSize = fileInfo.Length;
        }

        /// <summary>
        /// Hide default Constructor
        /// </summary>
        private FileOptimized()
        { }

        public FileOptimized(string name, long size, string fullPath, SpaceSavedAction action)
        {
            FullPath = fullPath;
            FileName = name;
            FileSize = size;
            SavedAction = action;
        }

        /// <summary>
        /// Formatted string of the File Optimized
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format($"File: {FileName} Size: {FileSize} was {SavedAction}");
        }
    }
}
