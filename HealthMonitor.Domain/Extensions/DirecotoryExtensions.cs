using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitor.Domain.Extensions
{
    public static class DirecotoryExtensions
    {
        public static FileInfo[] GetFiles(this DirectoryInfo directory, string[] patterns)
        {
            if (directory is null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            if (patterns is null)
            {
                throw new ArgumentNullException(nameof(patterns));
            }

            List<FileInfo> files = new List<FileInfo>();
            foreach (string searchPattern in patterns)
            {
                files.AddRange(directory.GetFiles(searchPattern));
            }
            return files.ToArray();
        }
        public static FileInfo GetFileRecursive(this DirectoryInfo directory, string pattern)
        {
            if (directory is null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            if (string.IsNullOrEmpty(pattern))
            {
                throw new ArgumentException($"'{nameof(pattern)}' cannot be null or empty", nameof(pattern));
            }

            FileInfo[] files = directory.GetFiles(pattern);
            if (files.Length > 0)
            {
                return files[0];
            }
            return (from subDirectory in directory.GetDirectories()
                    select subDirectory.GetFileRecursive(pattern)).FirstOrDefault((FileInfo file) => file != null);
        }
        public static FileInfo GetFileRecursive(this DirectoryInfo directory, Func<FileInfo, bool> predicate)
        {
            if (directory is null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            using (IEnumerator<FileInfo> enumeratior = directory.GetFiles().Where(predicate).GetEnumerator())
            {
                if (enumeratior.MoveNext())
                {
                    return enumeratior.Current;
                }
            }
            return (from subDirectory in directory.GetDirectories()
                    select subDirectory.GetFileRecursive(predicate)).FirstOrDefault((FileInfo file) => file != null);
        }
        public static long GetCurrentSize(this DirectoryInfo directory)
        {
            if (directory is null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            return directory.GetFiles().Sum((FileInfo x) => x.Length);
        }
        public static long GetCurrentSize(this DirectoryInfo directory, Func<FileInfo, bool> predicate)
        {
            if (directory is null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return directory.GetFiles().Where(predicate).Sum((FileInfo x) => x.Length);
        }
        public static IEnumerable<FileInfo> GetFiles(this DirectoryInfo directory, Func<FileInfo, bool> predicate)
        {
            if (directory is null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return directory.GetFiles().Where(predicate);
        }
        /// <summary>
        /// Get size of all the files in the Directory except the files pattern 
        /// </summary>
        /// <returns></returns>
        public static long GetCurrentSizeExceptFiles(this DirectoryInfo directory, string pattern)
        {
            if (directory is null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            if (string.IsNullOrEmpty(pattern))
            {
                throw new ArgumentException($"'{nameof(pattern)}' cannot be null or empty", nameof(pattern));
            }

            return directory.GetCurrentSize(x => !x.Extension.Equals(pattern));
        }
        /// <summary>
        /// Get all the files in the Directory except the  pattern 
        /// </summary>
        /// <param name="directory">The <see cref="DirectoryInfo"/> Directory under consideration</param>
        /// <param name="pattern">exclude files</param>
        /// <returns></returns>
        public static IEnumerable<FileInfo> GetFilesExceptFiles(this DirectoryInfo directory, string pattern)
        {
            if (directory is null)
            {
                throw new ArgumentNullException(nameof(directory));
            }

            if (string.IsNullOrEmpty(pattern))
            {
                throw new ArgumentException($"'{nameof(pattern)}' cannot be null or empty", nameof(pattern));
            }

            return directory.GetFiles().Where(x => !x.Extension.Equals(pattern));
        }
    }
}
