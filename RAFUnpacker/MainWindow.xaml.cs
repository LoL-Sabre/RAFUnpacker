using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.Collections;
using WPFFolderBrowser;
using System.IO;
using Microsoft.Win32;

namespace RAFUnpacker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private string LoLLocation = "C:\\";
        private Logger log;
        public MainWindow()
        {
            log = new Logger(DateTime.Now.ToString("HH-mm-ss"));
            InitializeComponent();
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\Wow6432Node\\Riot Games\\League of Legends"))
            {
                if (key != null)
                {
                    Object o = key.GetValue("Path");
                    LoLLocation = o.ToString();
                }
            }
        }

        private void buttonReadArchive_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.InitialDirectory = LoLLocation;
            if(ofd.ShowDialog() == true)
            {
                try
                {
                    RAFFile raf = new RAFFile(ofd.FileName, log, ofd.FileName);
                    dataFiles.ItemsSource = raf.Files;
                }
                catch(Exception) { }
            }
        }

        private void buttonExportSelected_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                IList selItem = dataFiles.SelectedItems;
                WPFFolderBrowserDialog fbd = new WPFFolderBrowserDialog();
                if (fbd.ShowDialog() == true)
                {
                    foreach (RAFFile.FileListEntry f in selItem)
                    {
                        Directory.CreateDirectory(fbd.FileName + "/" + System.IO.Path.GetDirectoryName(f.FileName));
                        var a = File.Create(fbd.FileName + "/" + System.IO.Path.GetDirectoryName(f.FileName) + "/" + System.IO.Path.GetFileName(f.FileName));
                        a.Dispose();
                        a.Close();
                        File.WriteAllBytes(fbd.FileName + "/" + System.IO.Path.GetDirectoryName(f.FileName) + "/" + System.IO.Path.GetFileName(f.FileName), f.Decompressed);
                    }
                    log.Write("RAF::IO::EXPORT | SUCESSFULLY EXPORTED ALL FILES", Logger.WriterType.WriteMessage);
                }
            }
            catch(Exception) { }
        }

        private void delLogs_Click(object sender, RoutedEventArgs e)
        {
            log.DeleteLogs();
        }
        private void buttonGit(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://github.com/LoL-Sabre/RAFUnpacker");
            }
            catch(Exception) { }
        }
    }
}
