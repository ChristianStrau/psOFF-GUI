using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace psOFF_GUI
{

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
