using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Shamsi_Photo_Organizer.Model;

namespace Shamsi_Photo_Organizer.Utils
{
    internal static class FileUtils
    {
        private static readonly String[] SupportedExtensions = {".jpg", ".jpeg"};

        public static List<string> GetPhotosListAsString(string dir) =>
            Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories)
                .Where(file => SupportedExtensions.Contains(Path.GetExtension(file)?.ToLowerInvariant()))
                .ToList();

        // for debug purpose
        private static void DateToFile(DateTime dateTime, string file)
        {
            var sb = new StringBuilder("\n\n======> " + file);
            sb.AppendLine();
            sb.AppendLine(dateTime.ToString(CultureInfo.InvariantCulture));
            sb.AppendLine();
            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\date_log.txt",
                sb.ToString());
        }


        public static void Rename(PhotoItem photoItem, string prefix)
        {
            if (!photoItem.Renamable) return;

            var newPath = photoItem.GetNewPath(prefix);

            Debug.WriteLine($"old_path: {photoItem.FullPath}");
            Debug.WriteLine($"new_path: {newPath}");
            Debug.WriteLine("");

            try
            {
                // File.Move(mediaItem.FullPath, newPath);
                Debug.WriteLine($"Rename: {newPath}");
            }
            catch (Exception e)
            {
                //TODO
                Debug.WriteLine($"error: {e.Message}");
            }
        }

        public static void Organize(PhotoItem photoItem, OrganizeMethod method)
        {
            if (!photoItem.Renamable) return;

            string newPath = photoItem.FileDir + Path.DirectorySeparatorChar;
            switch (method)
            {
                case OrganizeMethod.ByYear:
                    newPath += $"{photoItem.GetYear()}";
                    break;

                case OrganizeMethod.ByYearMonth:
                    newPath += $"{photoItem.GetYear()}-{photoItem.GetMonth()}";
                    break;

                case OrganizeMethod.ByMonthInYear:
                    newPath += $"{photoItem.GetYear()}{Path.DirectorySeparatorChar}{photoItem.GetMonth()}";
                    break;
            }

            newPath += Path.DirectorySeparatorChar;

            if (!Directory.Exists(newPath)) Directory.CreateDirectory(newPath);

            newPath += photoItem.FileName;
            try
            {
                //File.Move(photo.FullPath, newPath);
                Debug.WriteLine($"Move: {newPath}");
            }
            catch (Exception e)
            {
                //TODO
                Debug.WriteLine($"error: {e.Message}");
            }
        }

        //TODO add option to delete empty dirs
        public static void DeleteEmptyDirs(string startLocation)
        {
            foreach (var directory in Directory.GetDirectories(startLocation))
            {
                DeleteEmptyDirs(directory);
                if (Directory.GetFiles(directory).Length == 0 &&
                    Directory.GetDirectories(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                }
            }
        }
    }
}