using Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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

        public MainForm()
        {
            InitializeComponent();
        }

        private void MailForm_Load(object sender, EventArgs e)
        {
            
        }

        private void btnBrowseFile_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (DialogResult.OK == dialog.ShowDialog())
            {
                txtSourceFile.Text = dialog.FileName;

            }
        }

        private void btnBrowseFolder_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();

            if (false == string.IsNullOrEmpty(txtSourceFile.Text))
            {
                dialog.SelectedPath = new FileInfo(txtSourceFile.Text).DirectoryName;
            }

            if (DialogResult.OK == dialog.ShowDialog())
            {
                txtDestinationFolder.Text = dialog.SelectedPath;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            string sourceData = txtSourceFile.Text;
            string outputFolder = txtDestinationFolder.Text;

            if (string.IsNullOrEmpty(sourceData))
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
                var filler = new Filler();
                var json = filler.GetJson(sourceData);

                foreach (var j in json)
                {
                    string filePath = string.Format(@"{0}\{1}.json", outputFolder, j.Key);
                    File.WriteAllText(filePath, j.Value);
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

        private void txtSourceFile_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.All;
            else
                e.Effect = DragDropEffects.None;
        }

        private void txtSourceFile_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string file = files.FirstOrDefault();

            if (false == File.Exists(file)
                || new FileInfo(file).Extension.ToLower() != XLSX)
            {
                MessageBox.Show("File type should be .xlsx.");
                return;
            }

            txtSourceFile.Text = file;
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

    }
}
