using System;
using System.Collections.Generic;

namespace WeddingPlanner.Models
{
    public class Wrapper
    {
        public Wedding Wedding { get; set; }
        public List<Wedding> WeddingList { get; set; }
        public User User { get; set; }
    }
}