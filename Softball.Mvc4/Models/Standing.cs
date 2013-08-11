using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Softball.Mvc4.Models
{
    public class Standing
    {
        public int Place { get; set; }
        public string TeamName { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
    }
}