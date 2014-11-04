using Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SiteDataFiller
{
    public partial class MainForm : Form
    {
        private const string XLSX = ".xlsx";
        private const string SOURCE = "src";
        private const string DESTINATION = "dst";

        public MainForm()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.Configure();
        }

        private void MailForm_Load(object sender, EventArgs e)
        {
            txtSourceFolder.Text = ConfigurationManager.AppSettings[SOURCE];
            txtDestinationFolder.Text = ConfigurationManager.AppSettings[DESTINATION];
            LoadSourceFiles();
        }

        private void btnBrowseSource_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            if (DialogResult.OK == dialog.ShowDialog())
            {
                txtSourceFolder.Text = dialog.SelectedPath;

                LoadSourceFiles();
            }
        }

        private void btnBrowseDestination_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();

            if (false == string.IsNullOrEmpty(txtSourceFolder.Text))
            {
                dialog.SelectedPath = new DirectoryInfo(txtSourceFolder.Text).Parent.FullName;
            }

            if (DialogResult.OK == dialog.ShowDialog())
            {
                txtDestinationFolder.Text = dialog.SelectedPath;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            string sourceFolder = txtSourceFolder.Text;
            string outputFolder = txtDestinationFolder.Text;

            if (string.IsNullOrEmpty(sourceFolder))
            {
                MessageBox.Show("Please enter source data file location.");
                return;
            }

            if (string.IsNullOrEmpty(outputFolder))
            {
                MessageBox.Show("Please enter destination folder location.");
                return;
            }

            try
            {
                UpdateAppSettings(SOURCE, sourceFolder);
                UpdateAppSettings(DESTINATION, outputFolder);

                foreach (string srcFile in clbSourceFiles.CheckedItems)
                {
                    string[] splits = srcFile.Split(new char[] { '.' });
                    // data.en.xlsx
                    if (splits.Count() < 2) continue;

                    string lang = splits.ElementAt(splits.Count() - 2);

                    string sourceFile = Path.Combine(sourceFolder, srcFile);
                    string destFolder = Path.Combine(outputFolder, lang);

                    if (false == Directory.Exists(destFolder))
                        Directory.CreateDirectory(destFolder);

                    var filler = new Filler();
                    var json = filler.GetJson(sourceFile);

                    foreach (var j in json)
                    {
                        string filePath = string.Format(@"{0}\{1}.json", destFolder, j.Key);
                        File.WriteAllText(filePath, j.Value);
                    }
                }

                if (DialogResult.Yes == MessageBox.Show("Site data successfully exported.\n\r\n\rDo you want to close the aplication?", "Success", MessageBoxButtons.YesNo))
                {
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static void UpdateAppSettings(string key, string value)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                if (false == config.AppSettings.Settings.AllKeys.Contains(key))
                {
                    config.AppSettings.Settings.Add(key, value);
                }
                else
                {
                    config.AppSettings.Settings[key].Value = value;
                }

                config.Save(ConfigurationSaveMode.Full);
            }
            catch (Exception ex)
            {
            
            }
        }

        private void txtSourceFolder_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        private void txtSourceFolder_DragDrop(object sender, DragEventArgs e)
        {
            string[] dirs = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string dir = dirs.FirstOrDefault();

            if (false == Directory.Exists(dir))
            {
                MessageBox.Show("You should provide an existing directory path.");
                return;
            }

            txtSourceFolder.Text = dir;
        }

        private void txtDestinationFolder_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        private void txtDestinationFolder_DragDrop(object sender, DragEventArgs e)
        {
            string[] dirs = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string dir = dirs.FirstOrDefault();

            if (false == Directory.Exists(dir))
            {
                MessageBox.Show("You should provide an existing directory path.");
                return;
            }

            txtDestinationFolder.Text = dir;
        }

        private void LoadSourceFiles()
        {
            string sourceFolder = txtSourceFolder.Text;

            if (false == string.IsNullOrEmpty(sourceFolder))
            {
                var di = new DirectoryInfo(sourceFolder);
                var sourceFiles = di.EnumerateFiles()
                    .Where(f => f.Extension == ".xlsx")
                    .Where(f => f.Name.Count(c => c == '.') > 1)
                    .Select(f => f.Name)
                    .ToArray();

                clbSourceFiles.Items.Clear();
                clbSourceFiles.Items.AddRange(sourceFiles);
                for (int i = 0; i < clbSourceFiles.Items.Count; i++)
                    clbSourceFiles.SetItemChecked(i, true);
            }
        }

    }
}
