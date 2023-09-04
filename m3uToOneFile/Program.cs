using System;
using System.Collections.Generic;
using System.IO;
using NAudio.Wave;
using NAudio.Lame;

namespace AudioMerger
{
    class Program
    {
        static void Main(string[] args)
        {
            string directoryPath = "C:\\Users\\MSI PC\\Downloads\\";  // Directory containing the M3U files
            string[] m3uFiles = Directory.GetFiles(directoryPath, "*.m3u");  // Get all M3U files in the directory

            foreach (string m3uFilePath in m3uFiles)
            {
                string outputFileName = Path.GetFileNameWithoutExtension(m3uFilePath) + ".mp3";
                string outputFilePath = Path.Combine(directoryPath, outputFileName);
                string m3uDirectory = Path.GetDirectoryName(m3uFilePath);

                List<string> audioFiles = new List<string>();
                foreach (string line in File.ReadLines(m3uFilePath))
                {
                    if (!line.StartsWith("#"))
                    {
                        string absolutePath = Path.Combine(m3uDirectory, line.Trim());
                        audioFiles.Add(absolutePath);
                    }
                }

                MergeAudioFiles(audioFiles, outputFilePath);
            }
        }

        static void MergeAudioFiles(List<string> audioFiles, string outputFilePath)
        {
            using (var outputStream = new FileStream(outputFilePath, FileMode.Create))
            {
                using (var writer = new LameMP3FileWriter(outputStream, new WaveFormat(), LAMEPreset.STANDARD))
                {
                    foreach (string audioFile in audioFiles)
                    {
                        using (var reader = new Mp3FileReader(audioFile))
                        {
                            byte[] buffer = new byte[4096];
                            int bytesRead;
                            int totalBytesRead = 0;

                            while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                totalBytesRead += bytesRead;
                                writer.Write(buffer, 0, bytesRead);
                            }

                            Console.WriteLine("File {0} added to {1}", audioFile, outputFilePath);
                        }
                    }
                }
            }
        }
    }
}
