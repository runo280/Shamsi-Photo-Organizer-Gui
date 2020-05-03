using MetadataExtractor;
using Shamsi_Photo_Organizer.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Directory = System.IO.Directory;

namespace Shamsi_Photo_Organizer.Utils
{
    internal static class Utils
    {
        private static readonly string[] DateTimeFormats = {"yyyy:MM:dd HH:mm:ss"};

        private static readonly String[] SupportedExtensions = {".jpg", ".jpeg"};

        private static List<string> GetAllPhotosListAsString(string dir) =>
            Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories)
                .Where(file => SupportedExtensions.Contains(Path.GetExtension(file)?.ToLowerInvariant()))
                .ToList();

        public static List<Photo> GetAllPhotosList(string dir) => GetAllPhotosListAsString(dir).Select(file =>
        {
            var photo = new Photo(file);
            var dateTimeString = ExtractDateTimeFromMetadata(file);
            var result = dateTimeString.ToDate(DateTimeFormats);
            if (!result.HasValue) return photo;
            if (!result.Value.InRange()) return photo;
            photo.DateTimeString = dateTimeString;
            photo.DateTime = result.Value;
            photo.Renamable = true;
            return photo;
        }).ToList();

        public static int CountOfValidPhotos(List<Photo> photos) =>
            photos.FindAll(photo => photo.Renamable).Count;

        public static int CountOfPhotos(List<Photo> photos) =>
            photos.Count;

        private static string ExtractDateTimeFromMetadata(string file)
        {
            try
            {
                IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(file);
                return FindDateTimeFromDirectories(directories, out var dateTime) ? dateTime.Trim() : null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool FindDateTimeFromDirectories(IEnumerable<MetadataExtractor.Directory> directories,
            out string dateTime)
        {
            foreach (var directory in directories)
            {
                if (directory == null) continue;
                foreach (var tag in directory.Tags)
                {
                    if ((!tag.Name.ToLower().StartsWith("date/time"))) continue;
                    dateTime = tag.Description;
                    return true;
                }
            }

            dateTime = null;
            return false;
        }

        // for debug purpose
        private static void MetaDataToFile(IEnumerable<MetadataExtractor.Directory> directories, String file)
        {
            var sb = new StringBuilder("\n\n======> " + file);
            foreach (var directory in directories)
            {
                if (directory == null) continue;
                sb.AppendLine($"---> {directory.Name}");
                directory.Tags.Where(tag => tag != null).Select(tag => $"{tag.Name}: {tag.Description}")
                    .ToList().ForEach(line => sb.AppendLine(line));
            }

            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\log.txt",
                sb.ToString());
        }

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

        public static void RenamePhotos(List<Photo> photos, string prefix)
        {
            foreach (var photo in photos)
            {
                Rename(photo, prefix);
            }
        }

        private static void Rename(Photo photo, string prefix)
        {
            if (!photo.Renamable) return;

            var newPath = photo.GetNewPath(prefix);

            Debug.WriteLine($"old_path: {photo.FullPath}");
            Debug.WriteLine($"new_path: {newPath}");

            try
            {
                File.Move(photo.FullPath, newPath);
            }
            catch (Exception e)
            {
                //TODO
                Debug.WriteLine($"error: {e.Message}");
            }
        }


        public static void OrganizePhotos(List<Photo> photos, OrganizeMethod method)
        {
            foreach (var photo in photos)
            {
                Organize(photo, method);
            }
        }

        private static void Organize(Photo photo, OrganizeMethod method)
        {
            if (!photo.Renamable) return;

            string newPath = photo.FileDir + Path.DirectorySeparatorChar;
            switch (method)
            {
                case OrganizeMethod.ByYear:
                    newPath += $"{photo.GetYear()}";
                    break;

                case OrganizeMethod.ByYearMonth:
                    newPath += $"{photo.GetYear()}-{photo.GetMonth()}";
                    break;

                case OrganizeMethod.ByMonthInYear:
                    newPath += $"{photo.GetYear()}{Path.DirectorySeparatorChar}{photo.GetMonth()}";
                    break;
            }

            newPath += Path.DirectorySeparatorChar;

            if (!Directory.Exists(newPath)) Directory.CreateDirectory(newPath);

            newPath += photo.FileName;
            try
            {
                File.Move(photo.FullPath, newPath);
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