using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskListApplication.Models
{
    interface ITaskList
    {
        Boolean ToggleComplete(int id);
        Boolean Remove(int id);
        List<TaskEntry> GetRange(int offset, int count);
        Boolean RebuildList();
        Boolean Insert(string title);
    }
}
