using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using Shamsi_Photo_Organizer.Model;
using Directory = System.IO.Directory;

namespace Shamsi_Photo_Organizer.Utils
{
    internal static class Utils
    {
        private static readonly string[] DateTimeFormats = {"yyyy:MM:dd HH:mm:ss"};

        private static readonly String[] SupportedExtensions = {".jpg", ".jpeg"};

        private static List<string> GetPhotosListAsString(string dir) =>
            Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories)
                .Where(file => SupportedExtensions.Contains(Path.GetExtension(file)?.ToLowerInvariant()))
                .ToList();

        public static List<PhotoItem> GetPhotosList(string dir) => GetPhotosListAsString(dir).Select(file =>
        {
            var photo = new PhotoItem(file);
            var dateTimeString = ExtractDateTimeFromMetadata(file);
            var result = dateTimeString.ToDate(DateTimeFormats);
            Debug.WriteLine($"GetAllMediaList: {photo.FullPath + "\n" + dateTimeString}");
            if (!result.HasValue) return photo;
            if (!result.Value.InRange()) return photo;
            photo.DateTimeString = dateTimeString;
            photo.DateTime = result.Value;
            photo.Renamable = true;
            Debug.WriteLine($"GetAllMediaList: {photo.FullPath + "\n" + photo.DateTimeString}");
            return photo;
        }).ToList();

        public static int CountOfValidMedia(List<PhotoItem> photo) =>
            photo.FindAll(item => item.Renamable).Count;

        private static string ExtractDateTimeFromMetadata(string file)
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
        private static void MetaDataToFile(IEnumerable<MetadataExtractor.Directory> directories, String file)
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

        public static void RenamePhotos(List<PhotoItem> photos, string prefix)
        {
            foreach (var photo in photos)
            {
                Rename(photo, prefix);
            }
        }

        private static void Rename(PhotoItem photoItem, string prefix)
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

        public static void OrganizePhotos(List<PhotoItem> photos, OrganizeMethod method)
        {
            foreach (var photo in photos)
            {
                Organize(photo, method);
            }
        }

        private static void Organize(PhotoItem photoItem, OrganizeMethod method)
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