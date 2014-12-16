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
    public partial class NewWidgetForm : Form
    {
        protected Dictionary<string, string> _replacementsDictionary;
        public NewWidgetForm()
        {
            InitializeComponent();
        }

        public static bool ShowDialog(Dictionary<string, string> replacementsDictionary)
        {
            var form = new NewWidgetForm();

            form._replacementsDictionary = replacementsDictionary;

            var safeProjectName = replacementsDictionary["$safeprojectname$"].Replace(" ", ".");    //todo:::!::!:!!:
            form.txtServerNamespace.Text = safeProjectName;
            form.txtClientNamespace.Text = safeProjectName.ToLower();

            form.folderBrowserDialog1.SelectedPath = Properties.Settings.Default.VidereDir;
            form.txtVidereDir.Text = Properties.Settings.Default.VidereDir;

            return form.ShowDialog() == DialogResult.OK;
        }

        private bool validForm()
        {
            var err = "";
            var videreDir = Path.Combine(_replacementsDictionary["$destinationdirectory$"], txtVidereDir.Text);
            if (txtServerNamespace.Text.IndexOf(" ") > -1)    //todo:  use regex
                err = "Invalid Namespace";
            if (txtClientNamespace.Text.IndexOf(" ") > -1)    //todo:  use regex
                err = "Invalid Namespace";

            if (Directory.Exists(videreDir))
            {
                err = fileExists(videreDir, "bin\\Videre.Core.dll");
            }
            else
                err = string.Format("Videre Directory {0} does not exist.  Please select the location of your videre web installation", videreDir);

            if (!string.IsNullOrEmpty(err))
            {
                MessageBox.Show(err);
                return false;
            }
            return true;
        }


        private string fileExists(string dir, string fileName)
        {
            if (!File.Exists(Path.Combine(dir, fileName)))
                return string.Format("File does not exist ({0}).  Videre directory invalid.", fileName);
            return "";
        }

        private string fileVersion(string dir, string fileName, Version version)
        {
            var a = Assembly.LoadFile(Path.Combine(dir, fileName));
            if (a.GetName().Version < version)
                return string.Format("Version of file {0} needs to be version {1} or later.  Version found {2}.", fileName, version, a.GetName().Version);
            return "";
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtVidereDir.Text = makeRelative(folderBrowserDialog1.SelectedPath, _replacementsDictionary["$destinationdirectory$"]);
            }
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
                var videreDir = txtVidereDir.Text;

                Properties.Settings.Default.VidereDir = videreDir;
                _replacementsDictionary["$videredir$"] = videreDir;
                _replacementsDictionary["$servernamespace$"] = txtServerNamespace.Text;
                _replacementsDictionary["$clientnamespace$"] = txtClientNamespace.Text;

                Properties.Settings.Default.Save();
                this.DialogResult = DialogResult.OK;
                this.Dispose();
            }
        }

        private string makeRelative(string filePath, string referencePath)
        {
            if (referencePath.EndsWith("\\") == false)
                referencePath += "\\";
            var fileUri = new Uri(filePath);
            var referenceUri = new Uri(referencePath);
            return referenceUri.MakeRelativeUri(fileUri).ToString().Replace('/', Path.DirectorySeparatorChar);
        }
    }
}
