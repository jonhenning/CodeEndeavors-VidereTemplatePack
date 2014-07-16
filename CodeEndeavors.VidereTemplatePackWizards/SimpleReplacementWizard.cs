using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeEndeavors.VidereTemplatePackWizards
{
//$guid1$ = 225b359d-63e2-4875-8068-c544e89488ad;
//$guid2$ = 533ccb5a-6125-4b14-8e36-24d7e173aad3;
//$guid3$ = 1746e245-3cb8-4fe7-b810-a93d24103eac;
//$guid4$ = 4d108106-a99a-41e8-955e-3e99c2d07a13;
//$guid5$ = 4ee49dfd-85ba-4e00-9c2b-048297d39b35;
//$guid6$ = 0bcadea2-7368-49d3-9ef7-3a43b36c74a7;
//$guid7$ = cc98cfbc-86eb-4d4d-a478-c50e4a19c571;
//$guid8$ = 1d7f24c7-c968-41f6-9877-b5a32c272cf6;
//$guid9$ = e6a85bbc-c7f8-4a63-a609-e1ecbf99b99d;
//$guid10$ = 1f4db8ba-28b3-4ae1-8a4f-da73a8285643;
//$time$ = 2/21/2014 9:03:39 AM;
//$year$ = 2014;
//$username$ = Jon;
//$userdomain$ = ENDEAVORIII;
//$machinename$ = ENDEAVORIII;
//$clrversion$ = 4.0.30319.34011;
//$registeredorganization$ = ;
//$runsilent$ = False;
//$solutiondirectory$ = D:\dev\CodeEndeavors\OpenSource\VisualStudioTemplates\Sample1\ClassLibrary1\;
//$rootname$ = package3.manifest;
//$targetframeworkversion$ = 4.5;
//package3 = package3;
//$rootnamespace$ = ClassLibrary1;

    public class SimpleReplacementWizard : IWizard
    {
        private Dictionary<string, string> _replacementsDictionary = null;
        public void BeforeOpeningFile(EnvDTE.ProjectItem projectItem)
        {
            //throw new NotImplementedException();
        }

        public void ProjectFinishedGenerating(EnvDTE.Project project)
        {
            //throw new NotImplementedException();
        }

        public void ProjectItemFinishedGenerating(EnvDTE.ProjectItem projectItem)
        {
            //throw new NotImplementedException();
        }

        public void RunFinished()
        {
            //throw new NotImplementedException();
        }

        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            _replacementsDictionary = replacementsDictionary;
            _replacementsDictionary["$WizardType$"] = "SimpleReplacementWizard";
            //var s = "";
            //foreach (var key in _replacementsDictionary.Keys)
            //    s += key + " = " + _replacementsDictionary[key] + ";\r\n";
            //_replacementsDictionary["jon"] = s;
            //throw new NotImplementedException();
        }

        public bool ShouldAddProjectItem(string filePath)
        {
            //throw new NotImplementedException();
            return true;
        }
    }
}
