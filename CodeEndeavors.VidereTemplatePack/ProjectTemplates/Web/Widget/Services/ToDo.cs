using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace $safeprojectname$.Services
{
    public class ToDo
    {
        //TODO: this is just a sample in-memory database...  you will need to write a real-one!
        private static List<Models.Task> _tasks = new List<Models.Task>()
            {
                new Models.Task() { Id = 1, Name = "Laundry" },
                new Models.Task() { Id = 2, Name = "Wash Car", Description = "Get Billy to help!" },
                new Models.Task() { Id = 3, Name = "Bills", CompletedDate = new DateTime(2014, 1, 1) }
            };

        public static List<Models.Task> Get()
        {
            //todo:  real location call to data layer (Code First db access, Repository, etc.) - for now use memory
            return _tasks;
        }

        public static List<Models.Task> Save(Models.Task task)
        {
            //add persistance code here!
            var existingTask = _tasks.Where(t => t.Id == task.Id).FirstOrDefault();
            if (existingTask != null)
            {
                existingTask.Name = task.Name;
                existingTask.Description = task.Description;
                existingTask.CompletedDate = task.CompletedDate;
            }
            else 
            {
                task.Id = _tasks.Max(t => t.Id) + 1;
                _tasks.Add(task);
            }
            return _tasks;
        }

        public static List<Models.Task> Delete(int id)
        {
            //add persistance code here!
            var existingTask = _tasks.Where(t => t.Id == id).FirstOrDefault();
            if (existingTask != null)
                _tasks.Remove(existingTask);
            return _tasks;
        }

    }
}