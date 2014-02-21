using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Videre.Core.Models;
using CoreModels = Videre.Core.Models;
using CoreServices = Videre.Core.Services;

namespace $safeprojectname$
{
    public class Registration : IWidgetRegistration
    {
        public int Register()
        {
            // need to add a namespaced route to our ajax endpoint (if any)
            // IMPORTANT:  the namespace needs to be something that won't conflict with a potential page url
            RouteTable.Routes.MapRoute(
                "$servernamespace$_default",
                "$servernamespace$/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "$servernamespace$.Controllers" }
            );

            // The Register method is called during application start
            // your widget manifest should be registered here.  
            var updates = CoreServices.Update.Register(new List<CoreModels.WidgetManifest>()
            {
                new CoreModels.WidgetManifest() { Path = "$servernamespace$", Name = "ToDo", Title = "To Do", Category = "Sample"}
            });

            // If you wish to secure an AJAX endpoint, you need to register an Area and a Name.
            // An area is just a namespace (not a MVC area).  The Name is something to group one or more roles under.
            // Typically the admin role will be granted the rights by default.
            // See the LocationController for a commented line of code on how to ensure the AJAX call has rights
            //updates += CoreServices.Update.Register(new List<CoreModels.SecureActivity>()
            //{
            //    new CoreModels.SecureActivity() { PortalId = CoreServices.Portal.CurrentPortalId, Area = "$servernamespace$", Name = "Administration", Roles = new List<string>() {CoreServices.Update.AdminRoleId} }
            //});

            return updates; //return number of updates we have (so we can call save if something changed)
        }

        public int RegisterPortal(string portalId)
        {
            var updates = 0;
            // add registration logic that is specific to a portal here
            return updates;        
        }
    }
}
