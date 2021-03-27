using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthMonitor.Domain.SpaceOptimization;

namespace HealthMonitor.Services.Interfaces
{
    public interface ISpaceOptimizationService
    {
        int MaxDays { set; }
        long MaxSize { set; }
        SpaceSavedAction ActionRequired(FileInfo fileInfo);
        SpaceSavedAction ActionRequired(long fileSize, DateTime fileCreatedDate);
    }
}
