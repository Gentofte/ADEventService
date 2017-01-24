using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADEventService.Models
{
    // ================================================================================
    [Serializable]
    internal class CacheItem
    {
        public byte[] Value { get; set; }
        public DateTime ChangeTS { get; set; }
    }
}
