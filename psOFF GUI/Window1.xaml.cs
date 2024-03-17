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

namespace psOFF_GUI
{
    public partial class Window1 : Window
    {
        private string filePath;
        
        public Window1(string filePath)
        {
            InitializeComponent();
            // Set the filePath property
            this.filePath = filePath;

            // Call the method to set the image source
            SetImageSource();
        }

        private void SetImageSource()
        {
            testText.Text = "0";
            // Check if filePath is not empty
            if (!string.IsNullOrEmpty(filePath))
            {
                testText.Text = "1";
                try
                {
                    // Construct the path to the image dynamically based on the provided file path
                    string directoryPath = System.IO.Path.GetDirectoryName(filePath);
                    string imagePath = System.IO.Path.Combine(directoryPath, "sce_sys", "icon0.png");
                    testText.Text = imagePath; 
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
                    testText.Text = "2";
                    // Handle any exceptions
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RunEmulator(filePath);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ShortcutCreator.CreateShortcut("test.lnk", filePath);
        }


        public static void RunEmulator(string filePath)
        {
            try
            {
                // Create process start info
                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "cmd.exe";
                psi.RedirectStandardInput = true;
                psi.UseShellExecute = false;
                psi.CreateNoWindow = false;

                // Start the process
                Process process = Process.Start(psi);

                // Write command to cmd
                if (process != null)
                {
                    process.StandardInput.WriteLine(".\\emulator.exe --file=\"" + filePath + "\"");
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

        
    }


    public class ShortcutCreator
    {
        public static void CreateShortcut(string shortcutName, string filePath)
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
                shortcut.Arguments = $"/k cd /d \"{Environment.CurrentDirectory}\" & .\\emulator.exe --file=\"{filePath}\"";

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



