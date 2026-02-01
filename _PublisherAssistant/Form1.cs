using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace _PublisherAssistant
{
    public partial class Form1 : Form
    {
        private long totalFileCount = 0;
        private long totalByteSize = 0;

        public Form1()
        {
            InitializeComponent();
            Directory_Structure_ListBox.Font = new Font("Consolas", 10F, FontStyle.Regular);
        }

        private async Task ListDirectoryContentsAsync(string path, string rootPath)
        {
            try
            {
                string[] directories = await Task.Run(() => Directory.GetDirectories(path));
                string[] files = await Task.Run(() => Directory.GetFiles(path));

                foreach (string directory in directories)
                {
                    await ListDirectoryContentsAsync(directory, rootPath);
                }

                // rootPath: "C:\Users\...\Publish"
                // file:     "C:\Users\...\Publish\wwwroot\assets\css\app.css"
                // Sonuç:    "\wwwroot\assets\css\app.css"
                int trimLength = rootPath.Length;

                foreach (string file in files)
                {
                    FileInfo fInfo = new FileInfo(file);
                    totalFileCount++;
                    totalByteSize += fInfo.Length;

                    // Dosya yolunun başından rootPath kısmını atıyoruz
                    string displayPath = file.Substring(trimLength);

                    // Eğer path başında "\" yoksa ekliyoruz (Görsel tutarlılık için)
                    if (!displayPath.StartsWith("\\"))
                    {
                        displayPath = "\\" + displayPath;
                    }

                    Directory_Structure_ListBox.Items.Add(displayPath);
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception ex)
            {
                Directory_Structure_ListBox.Items.Add("[Error]: " + ex.Message);
            }
        }

        private async void Directory_Structure_btn_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select target directory";
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath;
                    Directory_Structure_txt.Text = selectedPath;

                    Directory_Structure_ListBox.Items.Clear();
                    totalFileCount = 0;
                    totalByteSize = 0;
                    Total_Number_Of_Files_Lbl.Text = "Files: 0";
                    Total_File_Size_Lbl.Text = "Size: 0 KB";

                    Directory_Structure_ListBox.BeginUpdate();
                    Directory_Structure_btn.Enabled = false;

                    // Seçilen yolu olduğu gibi gönderiyoruz
                    await ListDirectoryContentsAsync(selectedPath, selectedPath);

                    Total_Number_Of_Files_Lbl.Text = $"Total Files: {totalFileCount:N0}";
                    Total_File_Size_Lbl.Text = $"Total Size: {FormatSize(totalByteSize)}";

                    Directory_Structure_ListBox.EndUpdate();
                    Directory_Structure_btn.Enabled = true;
                }
            }
        }
        private string FormatSize(long bytes)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB" };
            if (bytes == 0) return "0 B";
            long bytesAbs = Math.Abs(bytes);
            int place = Convert.ToInt32(Math.Floor(Math.Log(bytesAbs, 1024)));
            double num = Math.Round(bytesAbs / Math.Pow(1024, place), 1);
            return (Math.Sign(bytes) * num).ToString() + " " + suf[place];
        }

        private void Web_Config_btn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.Title = "Select Web.Config file";
                fileDialog.Filter = "Configuration Files|web.config|All Files (*.*)|*.*";

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (Path.GetFileName(fileDialog.FileName).Equals("web.config", StringComparison.OrdinalIgnoreCase))
                    {
                        Web_Config_txt.Text = fileDialog.FileName;
                    }
                    else
                    {
                        MessageBox.Show("Please select a file named 'web.config'!", "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }
    }
}