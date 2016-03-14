using EnvDTE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CodeEndeavors.VidereTemplatePackWizards
{
    public partial class NewWidgetItemForm : Form
    {
        protected Dictionary<string, string> _replacementsDictionary;
        public NewWidgetItemForm()
        {
            InitializeComponent();
        }

        public static bool ShowDialog(Dictionary<string, string> replacementsDictionary, DTE dte)
        {
            var form = new NewWidgetItemForm();

            form._replacementsDictionary = replacementsDictionary;

            Array activeProjects = (Array)dte.ActiveSolutionProjects;
            if (activeProjects.Length > 0)
            {
                var activeProj = (Project)activeProjects.GetValue(0);
                
                var safeProjectName = activeProj.Properties.Item("RootNamespace").Value.Replace(" ", ".");    //todo:::!::!:!!:
                form.txtServerNamespace.Text = safeProjectName;
                replacementsDictionary["$servernamespace$"] = safeProjectName;
                //foreach (ProjectItem pi in activeProj.ProjectItems)
                //{
                    // Do something for the project items like filename checks etc.
                //}
            }

            //project.Properties.Item("DefaultNamespace").Value.ToString()

            var clientNamespace = replacementsDictionary["$safeitemname$"].Replace(" ", ".");    //todo:::!::!:!!:
            var clientClassName = clientNamespace;
            if (clientNamespace.IndexOf(".") > -1)
            {
                clientClassName = clientNamespace.Substring(clientNamespace.LastIndexOf(".") + 1);
                clientNamespace = clientNamespace.Substring(0, clientNamespace.LastIndexOf("."));
            }
            form.txtClientNamespace.Text = replacementsDictionary["$servernamespace$"].ToLower();//clientNamespace.ToLower();
            form._replacementsDictionary["$clientclassname$"] = clientClassName.ToLower();


            return form.ShowDialog() == DialogResult.OK;
        }

        private bool validForm()
        {
            var err = "";
            if (txtServerNamespace.Text.IndexOf(" ") > -1)    //todo:  use regex
                err = "Invalid Namespace";
            if (txtClientNamespace.Text.IndexOf(" ") > -1)    //todo:  use regex
                err = "Invalid Namespace";

            if (!string.IsNullOrEmpty(err))
            {
                MessageBox.Show(err);
                return false;
            }
            return true;
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Dispose();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.codeendeavors.com");

        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (validForm())
            {
                _replacementsDictionary["$servernamespace$"] = txtServerNamespace.Text;
                _replacementsDictionary["$clientnamespace$"] = txtClientNamespace.Text;

                Properties.Settings.Default.Save();
                this.DialogResult = DialogResult.OK;
                this.Dispose();
            }
        }

    }
}
