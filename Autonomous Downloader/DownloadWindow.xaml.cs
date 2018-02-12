using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Autonomous_Downloader
{
    /// <summary>
    /// Interaction logic for DownloadWindow.xaml
    /// </summary>
    public partial class DownloadWindow : Window
    {
        public DownloadWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

#if true
        private void DownloadBtn_Click(object sender, RoutedEventArgs e)
        {
            ftp ftpClient = new ftp(HostTB.Text, UserTB.Text, PasswordTB.Password);

            if (OperationSelectLB.SelectedIndex == 0)
            {
                ftpClient.upload(RemoteFilenameTB.Text, LocalFilenameTB.Text);
            }
            else
            {
                

                // ftpClient.upload(RemoteFilenameTB.Text, LocalFilenameTB.Text);
                string[] files = ftpClient.directoryListSimple(RemoteFilenameTB.Text);

                OutputTB.Clear();
                foreach (String filename in files)
                {
                    OutputTB.Text += String.Format("{0}\n", filename);
                }
            }
            OutputTB.Text += "Done";
        }
#else
        private void DownloadBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var ftpClient = new System.Net.FtpClient.FtpClient())
                {
                    var destPath = RemoteDirectoryTB.Text;
                    var sourcePath = LocalFilenameTB.Text;

                    ftpClient.Host = HostTB.Text;
                    ftpClient.Credentials = new System.Net.NetworkCredential(UserTB.Text, PasswordTB.Password);
                    ftpClient.EncryptionMode = System.Net.FtpClient.FtpEncryptionMode.None;
                    ftpClient.Connect();

                    using (var fileStream = File.OpenRead(sourcePath))
                    using (var ftpStream = ftpClient.OpenWrite(string.Format("{0}/{1}", destPath, System.IO.Path.GetFileName(sourcePath))))
                    {
                        var buffer = new byte[8 * 1024];
                        int count;
                        while ((count = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            ftpStream.Write(buffer, 0, count);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Typical exceptions here are IOException, SocketException, or a FtpCommandException
                //TODO catch some exceptions here and report them 
            }

        }
#endif

        private void ChooseFileBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = LocalFilenameTB.Text;
            openFileDialog1.Filter = "Autononmous Configuration Files (*.txt, *.json)|*.txt;*.json|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog().GetValueOrDefault(false) == true)
            {
                LocalFilenameTB.Text = openFileDialog1.FileName;
            }
        }
    }
}
