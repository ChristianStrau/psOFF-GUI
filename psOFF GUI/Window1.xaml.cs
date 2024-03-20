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
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using IWshRuntimeLibrary;
using System.Drawing;

namespace psOFF_GUI
{
    public partial class Window1 : Window
    {
        private string filePath;
        private string imagePath;
        string directoryPath = "schlecht";
        public Window1(string filePath)
        {
            InitializeComponent();
            // Set the filePath property
            this.filePath = filePath;
            imagePath = SetImageSource();
            // Call the method to set the image source
            SetImageSource();
            GetTitle.main(directoryPath);
        }

        private string SetImageSource() // Updated to return the image path
        {
            string imagePath = null; // Initialize imagePath variable
            // Check if filePath is not empty
            if (!string.IsNullOrEmpty(filePath))
            {
                try
                {
                    // Construct the path to the image dynamically based on the provided file path
                    string directoryPath = System.IO.Path.GetDirectoryName(filePath);
                    imagePath = System.IO.Path.Combine(directoryPath, "sce_sys", "icon0.png");
                    
                    // Set the source of the Image
                    if (System.IO.File.Exists(imagePath))
                    {
                        iconImage.Source = new BitmapImage(new Uri(imagePath));
                    }
                    else
                    {
                        // Handle case where image file does not exist
                        MessageBox.Show("Image file not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return imagePath; // Return the imagePath

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string args = "";
            if(inArgs.Text != "Enter arguments")
            {
                args = inArgs.Text;
            }
            RunEmulator(filePath, args);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(imagePath)) // Check if imagePath is not null or empty
            {
                string args = "";
                if (inArgs.Text != "Enter arguments")
                {
                    args = inArgs.Text;
                }
                ShortcutCreator.CreateShortcut("test", filePath, imagePath, args); // Pass the imagePath to CreateShortcut method
            }
            else
            {
                MessageBox.Show("Image path is empty.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public static void RunEmulator(string filePath, string arguments)
        {
            try
            {
                // Create process start info
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "cmd.exe";
                psi.RedirectStandardInput = true;
                psi.UseShellExecute = false;
                psi.CreateNoWindow = false;
                psi.Arguments = arguments;

                // Start the process
                Process process = Process.Start(psi);

                // Write command to cmd
                if (process != null)
                {
                    process.StandardInput.WriteLine(".\\emulator.exe --file=\"" + filePath + "\"" + " " + arguments);
                    process.StandardInput.Flush();
                    process.StandardInput.Close();
                    process.WaitForExit();
                }
                else
                {
                    Console.WriteLine("Failed to start cmd process.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }


    public class ShortcutCreator
    {
        public static void CreateShortcut(string shortcutName, string filePath, string imagePath, string arguments)
        {
            try
            {
                // Get the desktop folder path
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);

                // Create a WshShell object
                WshShell shell = new WshShell();

                // Create the shortcut object
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(System.IO.Path.Combine(desktopPath, shortcutName + ".lnk"));

                // Set the target path and arguments for the shortcut
                shortcut.TargetPath = "cmd.exe";
                shortcut.Arguments = $"/k cd /d \"{Environment.CurrentDirectory}\" & .\\emulator.exe --file=\"{filePath}\"" + " " + arguments;

                //Add Icon to shortcut
                string iconPath = System.IO.Path.GetDirectoryName(imagePath);
                iconPath = System.IO.Path.Combine(iconPath, "icon1.ico"); //get Destination Path
                using (FileStream stream = System.IO.File.OpenWrite(iconPath))
                {
                    Bitmap originalBitmap = (Bitmap)System.Drawing.Image.FromFile(imagePath); //Bitmap of the Image
                    Bitmap resizedBitmap = new Bitmap(256, 256); //Empty Bitmap for the Resolution
                    using (Graphics g = Graphics.FromImage(resizedBitmap)) // Combine Bitmaps
                    {
                        // cut Bitmap to targeted resolution
                        g.DrawImage(originalBitmap, 0, 0, 256, 256);
                    }
                    Icon.FromHandle(resizedBitmap.GetHicon()).Save(stream); //save bitmap with Ico format.
                }
                shortcut.IconLocation = iconPath; //Apply Icon

                // Set the description for the shortcut
                shortcut.Description = "Shortcut to run emulator with specified file";

                // Save the shortcut
                shortcut.Save();

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while creating the shortcut: " + ex.Message);
            }
        }
    }

}



