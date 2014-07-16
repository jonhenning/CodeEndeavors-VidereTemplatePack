using System;
using System.Collections.Generic;
using System.IO;
using EnvDTE;
using System.Windows.Forms;
using Microsoft.Win32;
using Microsoft.VisualStudio.TemplateWizard;

namespace CodeEndeavors.VidereTemplatePackWizards
{
    public class VidereItemWizard : Microsoft.VisualStudio.TemplateWizard.IWizard
    {
        private Dictionary<string, string> _replacementsDictionary = null;

        public void ProjectFinishedGenerating(Project project)
        {

        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, Microsoft.VisualStudio.TemplateWizard.WizardRunKind runKind, object[] customParams)
        {
            _replacementsDictionary = replacementsDictionary;
            if (NewWidgetItemForm.ShowDialog(replacementsDictionary, automationObject as DTE) == false)
                throw new WizardCancelledException("The wizard has been cancelled by the user.");
        }

        public void RunFinished() { }
        public void BeforeOpeningFile(ProjectItem projectItem) { }
        // This method is only called for item templates, not for project templates.
        public void ProjectItemFinishedGenerating(ProjectItem projectItem)
        {
            //var renamedProjectItems = new Dictionary<string, ProjectItem>();
            //renameProjectItem(projectItem, renamedProjectItems);
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }

        private void renameProjectItem(ProjectItem item, Dictionary<string, ProjectItem> renamedProjectItems)
        {
            var filePath = item.Properties.Item("FullPath").Value;
            var newPath = getReplacementString(filePath);
            if (filePath != newPath)
                renamedProjectItems[newPath] = item;
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
