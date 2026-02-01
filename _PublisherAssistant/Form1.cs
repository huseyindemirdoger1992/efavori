using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        private async void TestButton_Click(object sender, EventArgs e)
        {
            label2.Text = $"Action Start At: {DateTime.Now}";
            TestButton.Enabled = false;
            string serverUrl = "ftp://94.199.202.149/";
            string userName = "efavoriconnectionftp";
            string password = "FeneriAslanYedi0!";

            try
            {
                // 1. WebRequest oluştururken metodu ListDirectory olarak değiştiriyoruz
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(serverUrl);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(userName, password);

                // Güvenlik ayarları
                request.EnableSsl = true;
                request.UsePassive = true;
                request.KeepAlive = false;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, ssl) => true;

                // 2. Yanıtı ASENKRON olarak almayı dene
                using (FtpWebResponse response = (FtpWebResponse)await request.GetResponseAsync())
                {
                    // Buraya ulaştıysa bağlantı başarılıdır
                    this.ActionHistoryList.Items.Add("CONNECTION SUCCESSFUL!");
                    this.ActionHistoryList.Items.Add($"Server Status:{response.StatusDescription}");
                }
            }
            catch (WebException webEx)
            {
                // Hata varsa buraya düşer
                string status = (webEx.Response != null) ? ((FtpWebResponse)webEx.Response).StatusDescription : "Detay Yok";
                this.ActionHistoryList.Items.Add($"Hata: {webEx.Message} Server Response:{status}");
            }
            finally
            {
                TestButton.Enabled = true;
                label3.Text = $"Action Finish At: {DateTime.Now}";
                label4.Text = $"Total Time: {(DateTime.Now - DateTime.Parse(label2.Text.Replace("Action Start At: ", ""))).TotalSeconds} seconds";
            }
        }

        private async void button1_Click(object sender, EventArgs e)
        {

        }
    }
}