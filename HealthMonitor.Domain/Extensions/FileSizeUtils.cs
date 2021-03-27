using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HealthMonitor.Domain.Extensions
{
    public static class FileSizeUtils
    {
        public static long FormattedFileSize(long bytes, ref long fm)
        {
            if (bytes < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes));
            }
            if ((int)fm == 4)
            {
                return bytes;
            }
            if (bytes > 0 && bytes < 1024)
            {
                return bytes;
            }
            fm++;
            return FormattedFileSize(bytes / 1024, ref fm);
        }
        public static string FileSizeDescription(long bytes)
        {
            long val = 0;
            long num = FormattedFileSize(bytes, ref val);
            return $"{num:##.##}.{val}Kb";
        }
    }
}
