using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain
{
    public class BucketState
    {
        public int Step { get; set; }
        public int BucketX { get; set; }
        public int BucketY { get; set; }
        public required string Action { get; set; }
        public string? Status { get; set; }
    }
}
