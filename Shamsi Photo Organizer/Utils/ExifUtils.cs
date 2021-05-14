using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Directory = MetadataExtractor.Directory;

namespace Shamsi_Photo_Organizer.Utils
{
    internal static class ExifUtils
    {
        public static string FindDateTimeFromFile(string file)
        {
            try
            {
                var directories = ImageMetadataReader.ReadMetadata(file);
                var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                var date = subIfdDirectory?.GetDescription(ExifDirectoryBase.TagDateTimeOriginal) ??
                           subIfdDirectory?.GetDescription(ExifDirectoryBase.TagDateTimeDigitized) ??
                           subIfdDirectory?.GetDescription(ExifDirectoryBase.TagDateTime);
                return date;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        // for debug purpose
        public static void MetaDataToFile(IEnumerable<Directory> directories, String file)
        {
            var sb = new StringBuilder("\n\n======> " + file);
            foreach (var directory in directories)
            {
                if (directory == null) continue;
                sb.AppendLine($"---> {directory.Name}");
                directory.Tags.Select(tag => $"{tag.Name}: {tag.Description}")
                    .ToList().ForEach(line => sb.AppendLine(line));
            }

            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\log.txt",
                sb.ToString());
        }
        
        static void PrintDirectories(IEnumerable<Directory> directories, string method)
        {
            Console.WriteLine();
            Console.WriteLine("-------------------------------------------------");
            Console.Write(' ');
            Console.WriteLine(method);
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine();

            // Extraction gives us potentially many directories
            foreach (var directory in directories)
            {
                // Each directory stores values in tags
                foreach (var tag in directory.Tags)
                    Console.WriteLine(tag);

                // Each directory may also contain error messages
                foreach (var error in directory.Errors)
                    Console.Error.WriteLine("ERROR: " + error);
            }
        }
    }
}