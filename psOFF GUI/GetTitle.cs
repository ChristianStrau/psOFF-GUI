﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace psOFF_GUI
{
    public class GetTitle
    {
        public static string paramPath;

        public static string getTitle(string filePath)
        {
            Console.WriteLine(filePath);
            paramPath = GetParamPath(Path.GetDirectoryName(filePath));
            return ExtractTitle(ReadParam(paramPath));
        }

        private static string GetParamPath(string directoryPath)
        {
            paramPath = Path.Combine(directoryPath, "sce_sys", "param.sfo");
            return paramPath;
        }

        public static string ReadParam(string paramPath)
        {
            if (File.Exists(paramPath))
            {
                try
                {
                    // Open the param.sfo file as a binary reader
                    using (BinaryReader reader = new BinaryReader(File.Open(paramPath, FileMode.Open)))
                    {
                        // Seek to the position where the title offset is stored (0x08)
                        reader.BaseStream.Seek(0x08, SeekOrigin.Begin);

                        // Read the title offset (4 bytes, little-endian)
                        int titleOffset = reader.ReadInt32();

                        // Seek to the title offset position
                        reader.BaseStream.Seek(titleOffset, SeekOrigin.Begin);

                        // Read the title length (2 bytes, little-endian)
                        short titleLength = reader.ReadInt16();

                        // Read the title string
                        byte[] titleBytes = reader.ReadBytes(titleLength);

                        // Convert the title bytes to ASCII string
                        string title = System.Text.Encoding.ASCII.GetString(titleBytes);

                        return title;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    return "Error";
                }
            }
            else
            {
                Console.WriteLine("Error: param.sfo file not found");
                return "File not found";
            }
        }

        private static string ExtractTitle(string paramInfo)
        {
            string[] attributes = paramInfo.Split('?');

            foreach (string attribute in attributes)
            {
                if (attribute.StartsWith("TITLE:"))
                {
                    return attribute.Substring(6).Trim();
                }
            }

            return "Title not found";
        }
    }
}
