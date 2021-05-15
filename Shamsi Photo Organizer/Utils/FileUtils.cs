using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using Shamsi_Photo_Organizer.Model;

namespace Shamsi_Photo_Organizer.Utils
{
    internal static class FileUtils
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly String[] SupportedExtensions = {".jpg", ".jpeg"};

        public static List<string> GetPhotosListAsString(string dir) =>
            Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories)
                .Where(file => SupportedExtensions.Contains(Path.GetExtension(file)?.ToLowerInvariant()))
                .ToList();

        public static void Rename(PhotoItem photoItem, string prefix)
        {
            if (!photoItem.Renamable) return;

            var newPath = photoItem.GetNewPath(prefix);

            try
            {
                Logger.Debug("Rename::Old:: {file}", photoItem.FullPath);
                Logger.Debug("Rename::New:: {file}", newPath);

                string newFile = newPath;
                if (File.Exists(newPath))
                {
                    newFile = GetNextFileName(newPath);
                }

                File.Move(photoItem.FullPath, newFile);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Rename::");
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
                Logger.Debug("Move::Old:: {file}", photoItem.FullPath);
                Logger.Debug("Move::New:: {file}", newPath);

                string newFile = newPath;
                if (File.Exists(newPath))
                {
                    newFile = GetNextFileName(newPath);
                }

                File.Move(photoItem.FullPath, newFile);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Move::");
            }
        }

        private static string GetNextFileName(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            string pathName = Path.GetDirectoryName(fileName);
            if (pathName == null) return fileName;
            string fileNameOnly = Path.Combine(pathName, Path.GetFileNameWithoutExtension(fileName));
            int i = 0;
            // If the file exists, keep trying until it doesn't
            while (File.Exists(fileName))
            {
                i += 1;
                fileName = $"{fileNameOnly}({i}){extension}";
            }

            return fileName;
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