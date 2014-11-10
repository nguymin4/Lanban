using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lanban.Model
{
    public class Backlog
    {
        public int Project_ID { get; set; }
        public int Swimlane_ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Complexity { get; set; }
        public string Color { get; set; }

    }

    public class Task
    {
        public int Project_ID { get; set; }
        public int Swimlane_ID { get; set; }
        public int Backlog_ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<int> Work_estimation { get; set; }
        public string Color { get; set; }
        public string Due_date { get; set; }
    }
}