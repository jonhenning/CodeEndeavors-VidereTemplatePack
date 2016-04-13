using CodeEndeavors.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;
using Videre.Core.Models;
using CoreModels = Videre.Core.Models;
using CoreServices = Videre.Core.Services;
//using Client = Your ClientX dll;


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
            //registerServices();

            var updates = registerWidgets();
            //updates += registerMenus();
            //updates += registerTemplates();


            

            return updates; //return number of updates we have (so we can call save if something changed)
        }

        //only necessary for registering service endpoints
        //private void registerServices()
        //{
        //    Client.ServiceName.Register(ConfigurationManager.AppSettings.GetSetting("ServiceConnection", ""), ConfigurationManager.AppSettings.GetSetting("HttpServiceTimeout", 240000));
        
        //}

        private int registerWidgets()
        {
            var updates = 0;
            updates = CoreServices.Update.Register(new List<CoreModels.WidgetManifest>()
            {
                new CoreModels.WidgetManifest() { Path = "$servernamespace$", Name = "ToDo", Title = "To Do", Category = "Sample"}
            });
            return updates;
        }

        //private int registerMenus()
        //{
        //    var updates = 0;

        //    var appMenuId = CoreServices.Menu.RegisterMenu("AppMenu", null);
        //    var homeSideMenuId = CoreServices.Menu.RegisterMenu("HomeSideMenu", null);

        //    updates += CoreServices.Portal.RegisterLayoutTemplate("Home", "LayoutName", "Full Layout Name", new List<CoreModels.Widget>()
        //    {
        //        CoreServices.Portal.GetWidgetForTemplate("Top", "LayoutPath", new List<string>() { appMenuId }),
        //        CoreServices.Portal.GetWidgetForTemplate("Left", "LayoutPath", new List<string>() { homeSideMenuId })
        //    });

        //    updates += CoreServices.Menu.AddMenuItem("AppMenu", new MenuItem()
        //    {
        //        Text = "Home",
        //        Items = new List<MenuItem>()
        //            {
        //                new MenuItem() { Text = "Home", Url = "~/"},
        //                new MenuItem() { Text = "App1", Url = "~/app1/default"},
        //                new MenuItem() { Text = "App2", Url = "~/app2/default"}
        //            }
        //    }, 1);

        //    updates += CoreServices.Menu.AddMenuItem("HomeSideMenu", new MenuItem() { Text = "Home", Icon = "icon-home", Url = "~/" } );
        //    updates += CoreServices.Menu.AddMenuItem("HomeSideMenu", new MenuItem() { Text = "Admin", Icon = "icon-settings", Url = "~/admin/portal", RoleIds = new List<string>() {CoreServices.Update.GetAdminRoleId()} });

        //    Below is only necessary for home screen 
        //    Sets the home screen layout
        //    If this is not the home screen keep the below commented or remove it
        //    var homeLayoutId = CoreServices.Portal.GetLayoutTemplate("Home").Id;
        //    var homePage = CoreServices.Portal.GetPageTemplate("");
        //    if (homePage != null)
        //    {
        //        if (homePage.LayoutId != homeLayoutId)
        //        {
        //            homePage.LayoutId = homeLayoutId;
        //            CoreServices.Portal.Save(homePage);
        //        }
        //    }
        //    return updates;
        //}

        //private int registerTemplates()
        //{
        //    var updates = 0;
        //    updates += CoreServices.Portal.RegisterPageTemplate("Page Name", "URL path (Ex. pagename/pagenamepart2 which will be equivalent to ~/pagename/pagenamepart2", "NNR.Widgets.PowerNET.Example", "Location to place on template Ex. "Main"", "WidgetPath");

        //    return updates;
        //}

        public int RegisterPortal(string portalId)
        {
            var updates = 0;
            // add registration logic that is specific to a portal here

            // If you wish to secure an AJAX endpoint, you need to register an Area and a Name.
            // An area is just a namespace (not a MVC area).  The Name is something to group one or more roles under.
            // Typically the admin role will be granted the rights by default.
            // See the ToDoController for a commented line of code on how to ensure the AJAX call has rights
            //updates += CoreServices.Update.Register(new List<CoreModels.SecureActivity>()
            //{
            //    new CoreModels.SecureActivity() { PortalId = portalId, Area = "$safeprojectname$", Name = "Administration", RoleIds = new List<string>() {CoreServices.Update.GetAdminRoleId(portalId)} }
            //});

            


            return updates;
        }
    }
}
