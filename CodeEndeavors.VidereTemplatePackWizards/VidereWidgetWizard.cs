using System;
using System.Collections.Generic;
using System.IO;
using EnvDTE;
using System.Windows.Forms;
using Microsoft.Win32;
using Microsoft.VisualStudio.TemplateWizard;

namespace CodeEndeavors.VidereTemplatePackWizards
{
    public class VidereWidgetWizard : Microsoft.VisualStudio.TemplateWizard.IWizard
    {
        private Dictionary<string, string> _replacementsDictionary = null;

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

            CopyFiles(destPath, _replacementsDictionary["$videredir$"]);
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, Microsoft.VisualStudio.TemplateWizard.WizardRunKind runKind, object[] customParams)
        {
            _replacementsDictionary = replacementsDictionary;
            if (NewWidgetForm.ShowDialog(replacementsDictionary) == false)
                    throw new WizardCancelledException("The wizard has been cancelled by the user.");
        }

        public void RunFinished() { }
        public void BeforeOpeningFile(ProjectItem projectItem) { }
        // This method is only called for item templates, not for project templates.
        public void ProjectItemFinishedGenerating(ProjectItem projectItem) { }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }

        private void CopyFiles(string destPath, string viderePath)
        {
            var projDir = new FileInfo(destPath).Directory;
            var libDir = projDir.FullName + @"\lib";
            if (!Directory.Exists(libDir))
                Directory.CreateDirectory(libDir);
            SafeCopy(Path.Combine(viderePath, @"bin\Videre.Core.dll"), Path.Combine(libDir, "Videre.Core.dll"));
            SafeCopy(Path.Combine(viderePath, @"bin\CodeEndeavors.Extensions.dll"), Path.Combine(libDir, "CodeEndeavors.Extensions.dll"));
            
            SafeCopy(Path.Combine(viderePath, @"bin\System.Web.Mvc.dll"), Path.Combine(libDir, "System.Web.Mvc.dll"));

            var sourceTargetBuildPath = Path.Combine(viderePath, @"..\lib\MSBuildTargets\");
            var destTargetBuildPath = Path.Combine(libDir, "MSBuildTargets");

            //copy msbuild extensions to lib - TODO: installer could include these!
            //sourceTargetBuildPath = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Code Endeavors, LLC", "InstallPath", @"c:\").ToString();

            if (!Directory.Exists(destTargetBuildPath))
                Directory.CreateDirectory(destTargetBuildPath);

            SafeCopy(Path.Combine(sourceTargetBuildPath, "ICSharpCode.SharpZipLib.dll"), Path.Combine(destTargetBuildPath, "ICSharpCode.SharpZipLib.dll"), true);
            SafeCopy(Path.Combine(sourceTargetBuildPath, "MSBuild.Community.Tasks.dll"), Path.Combine(destTargetBuildPath, "MSBuild.Community.Tasks.dll"), true);
            SafeCopy(Path.Combine(sourceTargetBuildPath, "MSBuild.Community.Tasks.Targets"), Path.Combine(destTargetBuildPath, "MSBuild.Community.Tasks.Targets"), true);
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
