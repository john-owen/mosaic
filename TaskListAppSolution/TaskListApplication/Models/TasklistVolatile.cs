using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TaskListApplication.Models
{
    public class TasklistVolatile : ITaskList
    {
        List<TaskEntry> taskList = new List<TaskEntry>(); // the tasklist
        public TasklistVolatile()
        {
            this.RebuildList();
        }
        public List<TaskEntry> GetRange(int offset, int count)
        {
            List<TaskEntry> tasks = new List<TaskEntry>(); // subset of tasks to return
            // Check we have enough tasks in list
            if (taskList.Count() < offset) return tasks;

            if (taskList.Count() < (offset + count))
                count = taskList.Count() - offset;   

            try
            {
                tasks = taskList.GetRange(offset, count);
            } catch
            {
                //tasks is empty
                tasks = new List<TaskEntry>();
            }
            return tasks;
        }

        public bool Insert(string title)
        {
            Random random = new Random();
            int id = random.Next();
            taskList.Add(new TaskEntry(id , title , false));
            return true;
        }

        public bool RebuildList()
        {
            taskList.Clear();
            taskList.Add(new TaskEntry(1,"Make Coffee",false));
            taskList.Add(new TaskEntry(2,"Bake some bread",false));
            taskList.Add(new TaskEntry(3,"Update CV,False",false));
            taskList.Add(new TaskEntry(4,"Install visual studio",false));
            taskList.Add(new TaskEntry(5,"Drink some water",false));
            taskList.Add(new TaskEntry(6,"Get Some Milk",false));
            taskList.Add(new TaskEntry(7,"Go for a Jog",false));
            taskList.Add(new TaskEntry(8,"Feed the cat",false));
            taskList.Add(new TaskEntry(9,"Cook some dinner",false));
            taskList.Add(new TaskEntry(10,"Stretch",false));
            taskList.Add(new TaskEntry(11,"Fix computer",false));
            taskList.Add(new TaskEntry(12,"Purchase mouse",false));
            taskList.Add(new TaskEntry(13,"Refactor Code",false));
            taskList.Add(new TaskEntry(14,"Review C# style guide",false));
            taskList.Add(new TaskEntry(15,"Remove template bloat",false));
            taskList.Add(new TaskEntry(16,"Place js in bundle",false));
            taskList.Add(new TaskEntry(17,"Test program",false));
            return true;
        }

        public bool Remove(int id)
        {
            var entryToRemove = taskList.SingleOrDefault(taskEntry => taskEntry.id == id);
            if (entryToRemove != null)
                taskList.Remove(entryToRemove);
            return true;
        }

        public bool ToggleComplete(int id)
        {
            TaskEntry entryToToggle = taskList.SingleOrDefault(taskEntry => taskEntry.id == id);
            if (entryToToggle != null)
            {
                int index = taskList.IndexOf(entryToToggle);
                if (taskList[index].isComplete)
                    taskList[index].isComplete = false;
                else
                    taskList[index].isComplete = true;
            }
            return true;
        }
    }
}