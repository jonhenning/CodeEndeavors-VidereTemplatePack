using System.Collections.Generic;
using System.Web.Mvc;
using Videre.Core.ActionResults;
using Videre.Core.Services;
using Models = $safeprojectname$.Models;

namespace $safeprojectname$.Controllers
{
    public class ToDoController : Controller
    {
        public JsonResult<List<Models.Task>> GetTasks()
        {
            //API.Execute is a helper method that wraps all of your code in the proper exception handling for an AJAX call, so all you need to write is the actual business logic as a passed in delegate
            return API.Execute<List<Models.Task>>(r =>
            {
                //Security.VerifyActivityAuthorized("$safeprojectname$", "Administration");    //if we wanted to secure this call we would register the secure activity in the Registration.cs and uncomment this line
                r.Data = $safeprojectname$.Services.ToDo.Get();
            });
        }

        public JsonResult<List<Models.Task>> SaveTask(Models.Task task)
        {
            return API.Execute<List<Models.Task>>(r =>
            {
                //Security.VerifyActivityAuthorized("$safeprojectname$", "Administration");    //if we wanted to secure this call we would register the secure activity in the Registration.cs and uncomment this line
                r.Data = $safeprojectname$.Services.ToDo.Save(task);
            });
        }

        public JsonResult<List<Models.Task>> DeleteTask(int id)
        {
            return API.Execute<List<Models.Task>>(r =>
            {
                //Security.VerifyActivityAuthorized("$safeprojectname$", "Administration");    //if we wanted to secure this call we would register the secure activity in the Registration.cs and uncomment this line
                r.Data = $safeprojectname$.Services.ToDo.Delete(id);
            });
        }

    }
}
