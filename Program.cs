using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Drawing;
using System.Drawing.Imaging;

namespace LCSDMS_Images_Migration
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Source directory
                string sourceDir = ConfigurationManager.AppSettings["ImagesSynedPath"].ToString();
                // Destination directory on the network
                var destinationDirAll = ConfigurationManager.AppSettings["Serverdirectorypath"].ToString().Split(',');
                foreach (var destinationDir in destinationDirAll)
                {
                    // Copy folders from source to destination
                    string[] folders = Directory.GetDirectories(sourceDir);
                    foreach (string folder in folders)
                    {
                        string folderName = new DirectoryInfo(folder).Name;
                        if (folderName.Contains("LCS"))
                        {
                            string destinationPath = Path.Combine(destinationDir, folderName);
                            using (new NetworkConnection(destinationDir.ToString(), new NetworkCredential(ConfigurationManager.AppSettings["User"].ToString(), ConfigurationManager.AppSettings["Pass"].ToString())))
                            {
                                if (Directory.Exists(destinationPath))
                                {
                                    Directory.Delete(destinationPath, true);
                                }

                                CopyFolder(folder, destinationPath);
                            }
                        }

                    }
                }
                Console.WriteLine("All folders copied successfully.");
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

        }
        static void CopyFolder(string sourceFolder, string destinationFolder)
        {
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            string[] files = Directory.GetFiles(sourceFolder);
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string destinationPath = Path.Combine(destinationFolder, fileName);
                try
                {
                    Image image = Image.FromFile(file);
                    image.Save(destinationPath, ImageFormat.Bmp); // Change format as needed
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error copying image: {ex.Message}");
                }
               // File.Copy(file, destinationPath, true);
            }

            string[] subfolders = Directory.GetDirectories(sourceFolder);
            foreach (string subfolder in subfolders)
            {
                string folderName = new DirectoryInfo(subfolder).Name;
                string newDestinationFolder = Path.Combine(destinationFolder, folderName);
                CopyFolder(subfolder, newDestinationFolder);
            }
        }
    }
}
