using System;

namespace Lanban.Model
{
    public class Backlog
    {
        // Backlog_ID nullable means that the project is not existed and will be created
        public Nullable<int> Backlog_ID { get; set; }
        public int Project_ID { get; set; }
        public int Swimlane_ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Complexity { get; set; }
        public string Color { get; set; }
        public string Start_date { get; set; }
        public string Completion_date { get; set; }
    }

    public class Task
    {
        // Task_ID nullable means that the project is not existed and will be created
        public Nullable<int> Task_ID { get; set; }
        public int Project_ID { get; set; }
        public int Swimlane_ID { get; set; }
        public int Backlog_ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Nullable<int> Work_estimation { get; set; }
        public string Color { get; set; }
        public string Due_date { get; set; }
        public string Completion_date { get; set; }
        public Nullable<int> Actual_work { get; set; }
    }

    public class ProjectModel
    {
        // Project_ID nullable means that the project is not existed and will be created
        public Nullable<int> Project_ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Owner { get; set; }
        public string Start_Date { get; set; }
        public string Modified_date { get; set; }
    }

    public class Swimlane
    {
        // Swimlane_ID nullable means that the swimlane is not existed and will be created
        public Nullable<int> Swimlane_ID { get; set; }
        public int Project_ID { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string Data_status { get; set; }
    }
}