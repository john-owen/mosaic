using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskListApplication.Models
{
    public class TaskEntry
    {
        public int id { get; set; }
        public string title { get; set; }
        public bool isComplete { get; set; }

        public TaskEntry(int id, string title, bool isComplete)
        {
            this.id = id;
            this.title = title;
            this.isComplete = isComplete;
        }
    }
}