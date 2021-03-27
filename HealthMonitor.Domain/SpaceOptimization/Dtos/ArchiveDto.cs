using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitor.Domain.SpaceOptimization.Dtos
{
    public class ArchiveDto
    {
        public string FullName { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public long Size { get; private set; }

        public ArchiveDto(string fullName, DateTime createdDate)
        {
            FullName = fullName;
            CreatedDate = createdDate;
        }

        public ArchiveDto(string fullName, DateTime createdDate, long size) : this(fullName, createdDate)
        {
            Size = size;
        }
    }
}
