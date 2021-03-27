using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Domain.Extensions;

namespace HealthMonitor.Domain.SpaceOptimization
{
    /// <summary>
    /// Class that summarized the number of files for space saving
    /// </summary>
    /// <remarks>
    /// SpaceOptimizationSummary.cs
    /// </remarks>
    public class SpaceOptimizationSummary
    {
        /// <summary>
        /// Used to start stopwatch
        /// </summary>
        private readonly DateTime startTime;
        /// <summary>
        /// Original size of the directory in bytes
        /// </summary>
        public long OriginalSize { get; set; }

        /// <summary>
        /// The current size of the directory after space optimization is performed
        /// </summary>
        public long CurrentSize { get; set; }
        /// <summary>
        /// Total Space Saved
        /// </summary>
        public long SpaceSaved { get { return OriginalSize - CurrentSize; } }
        /// <summary>
        /// Files that were manipulated to optimize space
        /// </summary>
        public List<FileOptimized> FilesOptimized { get; set; }
        /// <summary>
        /// The directory that will be optimized for space
        /// </summary>
        public string Directory { get; private set; }

        /// <summary>
        /// The time to take to optimize the directory
        /// </summary>
        public TimeSpan TimeToOptimizeSpace
        {
            get { return DateTime.Now - startTime; }
        }

        /// <summary>
        /// Hide default constructor
        /// </summary>
        private SpaceOptimizationSummary()
        { }

        /// <summary>
        /// Constructor for SpaceOptimizationSummary
        /// </summary>
        /// <param name="directory">The directory where space is going to be optimized</param>
        public SpaceOptimizationSummary(DateTime startTime, List<FileOptimized> filesOptimized, string directory)
        {
            this.startTime = startTime;
            FilesOptimized = filesOptimized;
            Directory = directory;
        }

        public SpaceOptimizationSummary(DateTime startTime, List<FileOptimized> filesOptimized, string directory, long originalSize)
            : this(startTime, filesOptimized, directory)
        {
            OriginalSize = originalSize;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendNewLine($"{Directory}");
            sb.AppendNewLine("========================================");
            sb.AppendNewLine($"Original Size: {OriginalSize}, Current Size: {CurrentSize}, Space Saved: {SpaceSaved}");
            foreach (var item in FilesOptimized)
            {
                sb.AppendNewLine($"   {item}");
            }
            sb.AppendNewLine($"Time to complete optimization {TimeToOptimizeSpace}");
            sb.AppendNewLine("========================================");
            return sb.ToString();
        }
    }
}
