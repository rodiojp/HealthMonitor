using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Domain.SpaceOptimization.Dtos;

namespace HealthMonitor.WindowsServices.Interfaces
{
    public interface IArchiveService
    {
        IEnumerable<ArchiveDto> GetArchiveFiles();
        void AddFile(KeyValuePair<string, DateTime> file);
        void AddFiles(IDictionary<string, DateTime> files);
        void DeleteFile(string fileName);
        string ArchiveFileName { set; }
    }
}
