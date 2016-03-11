using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using EnvDTE;
using System.Windows.Forms;
using Microsoft.Win32;
using Microsoft.VisualStudio.TemplateWizard;
using System.Reflection;
using EnvDTE80;
using EnvDTE100;
using TemplateBuilder;
using System.Threading;
using System.Linq;
using Microsoft.Web.Administration;
using System.Security.AccessControl;
using PSHostsFile;


namespace CodeEndeavors.VidereTemplatePackWizards
{
    public class VidereWidgetWizard : Microsoft.VisualStudio.TemplateWizard.IWizard
    {
        private Dictionary<string, string> _replacementsDictionary = null;

        private DTE2 _dte2;
        private Solution4 _solution;
        //private IList<Project> _existingProjects;

        public void ProjectFinishedGenerating(Project project)
        {
            //todo:  more replacement items to put here?
            //if (!_replacementsDictionary.ContainsKey("$rootnamespace$"))
            //    _replacementsDictionary["$rootnamespace$"] = project.Properties.Item("RootNamespace").Value;
            //if (!_replacementsDictionary.ContainsKey("$lowerrootnamespace$"))
            //    _replacementsDictionary["$lowerrootnamespace$"] = project.Properties.Item("RootNamespace").Value.ToLower();

            var destPath = _replacementsDictionary["$destinationdirectory$"];
            renameFolders(destPath);
            renameProjectItems(project);

            CopyFiles(destPath);
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, Microsoft.VisualStudio.TemplateWizard.WizardRunKind runKind, object[] customParams)
        {
            _replacementsDictionary = replacementsDictionary;

            _dte2 = automationObject as DTE2;
            if (_dte2 != null) _solution = (Solution4)_dte2.Solution;

            if (NewWidgetForm.ShowDialog(replacementsDictionary) == false)
                    throw new WizardCancelledException("The wizard has been cancelled by the user.");

            //_existingProjects = getProjects() ?? new Project[0];
        }

        public void RunFinished() 
        {
            if (_solution == null)
            {
                throw new Exception("No solution found.");
            }

            var projects = getProjects().ToList();//.Except(_existingProjects).ToList();
            if (projects == null || !projects.Any()) throw new Exception("No projects found.");

            ////projects = projects.OrderByDescending(p => p.Name).ToList();    //don't copy the one that matches the same name until END... LOSE ENTIRE FOLDER STRUCTURE THIS WAY

            ////Get the projects directory from first project.
            var originalProjectsDir = Path.GetDirectoryName(projects.First().FullName);
            if (originalProjectsDir == null) return;
            var tempProjectsDir = originalProjectsDir + ".temp";

            var solutionDir = new DirectoryInfo(originalProjectsDir).Parent.FullName;
            ////var solutionStructure = projects.Select(p => new KeyValuePair<Project, string>(null, Path.Combine(solutionDir, Path.GetFileNameWithoutExtension(Path.GetFileName(p.FullName))))).ToList();
            var newSolutionProjects = projects.Select(p => Path.Combine(Path.Combine(solutionDir, Path.GetFileNameWithoutExtension(Path.GetFileName(p.FullName))), Path.GetFileName(p.FullName))).ToList();

            var slnFileName = Path.Combine(solutionDir, new DirectoryInfo(originalProjectsDir).Name + ".sln");
            _solution.SaveAs(slnFileName);

            //get project references
            var projectReferences = new Dictionary<string, List<string>>();
            foreach (var project in projects)
            {
                projectReferences[project.Name] = new List<string>();

                var vsProj = project.Object as VSLangProj.VSProject;
                foreach (VSLangProj.Reference reference in vsProj.References)
                {
                    if (reference.SourceProject != null)
                        projectReferences[project.Name].Add(reference.SourceProject.Name);
                }
            }

            //Remove the projects from the solution
            foreach (var project in projects)
            {
                _solution.Remove(project);
                _solution.SaveAs(slnFileName);
            }

            //move the project dir to temp dir (we have same name between our projects dir and the service dir)
            Directory.Move(originalProjectsDir, tempProjectsDir);
            copyDirectory(tempProjectsDir, originalProjectsDir);    //move all contents of directory into original dir

            //Restructure solution
            foreach (var projectFile in newSolutionProjects)
            {
                _solution.AddFromFile(projectFile, false);
                _solution.SaveAs(slnFileName);
            }

            //add project references back
            var newProjects = getProjects();
            var projectDict = newProjects.ToDictionary(p => p.Name);
            foreach (var project in newProjects)
            {
                var vsProj = project.Object as VSLangProj.VSProject;
                foreach (var projectName in projectReferences[project.Name])
                    vsProj.References.AddProject(projectDict[projectName]);
            }

            Directory.Delete(tempProjectsDir, true);  //remove 
        }

        public void BeforeOpeningFile(ProjectItem projectItem) { }
        // This method is only called for item templates, not for project templates.
        public void ProjectItemFinishedGenerating(ProjectItem projectItem) { }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }

        private void CopyFiles(string destPath)
        {
            var viderePath = Path.Combine(_replacementsDictionary["$destinationdirectory$"], _replacementsDictionary["$videredir$"]);
            var projDir = new FileInfo(destPath).Directory;
            var libDir = projDir.FullName + @"\lib";
            if (!Directory.Exists(libDir))
                Directory.CreateDirectory(libDir);
            var destTargetBuildPath = Path.Combine(libDir, "MSBuildTargets");

            if (!Directory.Exists(destTargetBuildPath))
                Directory.CreateDirectory(destTargetBuildPath);
            var sourceTargetBuildPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "lib");

            SafeCopy(Path.Combine(sourceTargetBuildPath, "ICSharpCode.SharpZipLib.dll"), Path.Combine(destTargetBuildPath, "ICSharpCode.SharpZipLib.dll"), true);
            SafeCopy(Path.Combine(sourceTargetBuildPath, "MSBuild.Community.Tasks.dll"), Path.Combine(destTargetBuildPath, "MSBuild.Community.Tasks.dll"), true);
            SafeCopy(Path.Combine(sourceTargetBuildPath, "MSBuild.Community.Tasks.Targets"), Path.Combine(destTargetBuildPath, "MSBuild.Community.Tasks.Targets"), true);

            var videreDir = viderePath;
            var sourceViderePortal = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"ProjectTemplates\Web\ViderePortalZip");
            if (_replacementsDictionary.ContainsKey("CreateVidereWeb") && _replacementsDictionary["CreateVidereWeb"] == "true")
            {
                copyDirectory(sourceViderePortal, videreDir);
                var videreInstallPath = Path.Combine(videreDir, "Videre.Core.Web.NewInstall.zip");
                ZipFile.ExtractToDirectory(videreInstallPath, videreDir);
                File.Delete(videreInstallPath); //remove the zip file

                var url = _replacementsDictionary["$viderewebsite$"];
                var uri = new Uri(url);
                var name = uri.Host;
                var resolvedServiceHostDir = new DirectoryInfo(videreDir).FullName;
                createSite(resolvedServiceHostDir, name, name, name);

                if (PSHostsFile.HostsFile.Get().Where(e => e.Hostname.Equals(name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault() == null)
                {
                    if (MessageBox.Show("Do you wish to add a HOSTS file entry for " + name + "?", "Add Hosts Entry", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        PSHostsFile.HostsFile.Set(name, "127.0.0.1");
                }
                
                setFolderPermissions(videreDir, @"iis apppool\" + name);
                setFolderPermissions(videreDir, @"IUSR");
            }

        }

        private static void createSite(string directoryPath, string siteName, string host, string poolName)
        {
            using (var serverManager = new ServerManager())
            {
                if (serverManager.ApplicationPools[poolName] == null)
                {
                    var newPool = serverManager.ApplicationPools.Add(poolName);
                    newPool.ManagedRuntimeVersion = "v4.0";
                    serverManager.CommitChanges();
                }

                if (serverManager.Sites[siteName] == null)
                {
                    //bindingInformation = Address:Port:Host
                    var bindingInformation = string.Format("{0}:{1}:{2}", "*", "80", host);
                    var newSite = serverManager.Sites.Add(siteName, "http", bindingInformation, directoryPath);
                    newSite.ApplicationDefaults.ApplicationPoolName = poolName;

                    //var app = newSite.Applications.Add("/" + applicationName, directoryPath);
                    //app.ApplicationPoolName = poolName;
                    serverManager.CommitChanges();
                }
            }
        }

        private static void setFolderPermissions(string dirPath, string user)
        {
            var dir = new DirectoryInfo(dirPath);
            var ds = dir.GetAccessControl();
            ds.AddAccessRule(new FileSystemAccessRule(user,
            FileSystemRights.FullControl,
            InheritanceFlags.ObjectInherit |
            InheritanceFlags.ContainerInherit,
            PropagationFlags.None,
            AccessControlType.Allow));
            dir.SetAccessControl(ds);
        }

        private void SafeCopy(string from, string to, bool bypassError = false)
        {
            try
            {
                File.Copy(from, to, true);
            }
            catch (Exception ex)
            {
                if (bypassError == false)
                    MessageBox.Show(ex.Message);    //keep going!
            }
        }

        private void safeCopy(string from, string to, bool bypassError = false)
        {
            try
            {
                File.Copy(from, to, true);
            }
            catch (Exception ex)
            {
                //if (bypassError == false)
                MessageBox.Show(ex.Message);    //keep going!
            }
        }

        private void copyDirectory(string sourceFolder, string destinationFolder)
        {
            if (!Directory.Exists(destinationFolder))
                Directory.CreateDirectory(destinationFolder);

            var dirInfo = new DirectoryInfo(sourceFolder);
            var files = dirInfo.GetFiles();
            foreach (var tempfile in files)
            {
                if (!File.Exists(Path.Combine(destinationFolder, tempfile.Name)))
                    tempfile.CopyTo(Path.Combine(destinationFolder, tempfile.Name), false);
            }

            var directories = dirInfo.GetDirectories();
            foreach (var tempdir in directories)
                copyDirectory(Path.Combine(sourceFolder, tempdir.Name), Path.Combine(destinationFolder, tempdir.Name));

        }

        private IList<Project> getProjects()
        {
            var projects = _solution.Projects;
            var list = new List<Project>();
            var item = projects.GetEnumerator();

            while (item.MoveNext())
            {
                var project = item.Current as Project;
                if (project == null)
                    continue;

                if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                    list.AddRange(getSolutionFolderProjects(project));
                else
                    list.Add(project);
            }
            return list;
        }

        private static IEnumerable<Project> getSolutionFolderProjects(Project solutionFolder)
        {
            var list = new List<Project>();
            for (var i = 1; i <= solutionFolder.ProjectItems.Count; i++)
            {
                var subProject = solutionFolder.ProjectItems.Item(i).SubProject;
                if (subProject == null)
                    continue;

                // If this is another solution folder, do a recursive call, otherwise add
                if (subProject.Kind == EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder)
                    list.AddRange(getSolutionFolderProjects(subProject));
                else
                    list.Add(subProject);
            }
            return list;
        }


        private void renameProjectItems(Project project)
        {
            var renamedProjectItems = new Dictionary<string, ProjectItem>();
            renameProjectItems(project.ProjectItems, renamedProjectItems);
            foreach (var newFileName in renamedProjectItems.Keys)
            {
                renamedProjectItems[newFileName].Remove();
                if (System.IO.File.Exists(newFileName))
                    project.ProjectItems.AddFromFile(newFileName);
            }
        }

        private void renameProjectItems(ProjectItems items, Dictionary<string, ProjectItem> renamedProjectItems)
        {
            foreach (ProjectItem item in items)
            {
                var filePath = item.Properties.Item("FullPath").Value;
                var newPath = getReplacementString(filePath);
                if (filePath != newPath)
                    renamedProjectItems[newPath] = item;

                renameProjectItems(item.ProjectItems, renamedProjectItems); //recursively call

            }
        }

        private void renameFolders(string dir)
        {
            foreach (var folderName in Directory.GetDirectories(dir))
            {
                var folder = new DirectoryInfo(folderName);
                if (_replacementsDictionary.ContainsKey(folder.Name))
                {
                    var newName = folder.FullName.Replace(folder.Name, _replacementsDictionary[folder.Name]);
                    Directory.Move(folderName, newName);
                    renameFolders(newName); //recursively call
                }
                else
                    renameFolders(folderName); //recursively call
            }
        }

        private string getReplacementString(string val)
        {
            var newVal = val;
            foreach (var key in _replacementsDictionary.Keys)
                newVal = newVal.Replace(key, _replacementsDictionary[key]);
            return newVal;
        }

    }
}
