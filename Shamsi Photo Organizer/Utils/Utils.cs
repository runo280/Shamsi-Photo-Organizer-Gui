using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using MetadataExtractor;
using Shamsi_Photo_Organizer.Model;
using Directory = System.IO.Directory;

namespace Shamsi_Photo_Organizer.Utils
{
    class Utils
    {
        private static readonly string[] DateTimeFormats = {"yyyy:MM:dd HH:mm:ss"};

        private static List<string> getPhotosList(string dir)
        {
            var ext = new[] {".jpg", ".jpeg"};
            return Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories)
                .Where(file => ext.Contains(Path.GetExtension(file)?.ToLowerInvariant())).ToList();
        }

        public static List<Photo> getPhotos(string dir)
        {
            var photos = new List<Photo>();
            foreach (var file in getPhotosList(dir))
            {
                processPhoto(file, out var ph);
                photos.Add(ph);
            }

            return photos;
        }

        private static void processPhoto(string file, out Photo photo)
        {
            photo = new Photo(file);
            var dateTimeString = extractDateTimeFromMetadata(file);
            var result = dateTimeString.ToDate(DateTimeFormats);
            if (!result.HasValue) return;
            // dateToFile(result.Value, file);
            if (!result.Value.inRange()) return;
            photo.DateTimeString = dateTimeString;
            photo.DateTime = result.Value;
            photo.Renamable = true;
        }

        public static int getValidPhotosCount(List<Photo> photos)
        {
            return photos.FindAll(photo => photo.Renamable).Count;
        }

        private static string extractDateTimeFromMetadata(string file)
        {
            try
            {
                IEnumerable<MetadataExtractor.Directory> directories = ImageMetadataReader.ReadMetadata(file);
                return findDateTimeFromDirectories(directories, out var dateTime) ? dateTime.Trim() : null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private static bool findDateTimeFromDirectories(IEnumerable<MetadataExtractor.Directory> directories,
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
        private static void metaDataToFile(IEnumerable<MetadataExtractor.Directory> directories, String file)
        {
            var sb = new StringBuilder("\n\n======> " + file);
            foreach (var directory in directories)
            {
                if (directory == null) continue;
                sb.AppendLine($"---> {directory.Name}");
                foreach (var tag in directory.Tags)
                {
                    if (tag == null) continue;
                    sb.AppendLine($"{tag.Name}: {tag.Description}");
                }
            }

            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\log.txt",
                sb.ToString());
        }

        // for debug purpose
        private static void dateToFile(DateTime dateTime, string file)
        {
            var sb = new StringBuilder("\n\n======> " + file);
            sb.AppendLine();
            sb.AppendLine(dateTime.ToString(CultureInfo.InvariantCulture));
            sb.AppendLine();
            File.AppendAllText(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\date_log.txt",
                sb.ToString());
        }

        public static void organizeFile(string dir, string newOutPath, string prefix, bool rename, bool copy,
            OrganizeMethod method)
        {
            foreach (var photo in getPhotos(dir))
            {
                _organizeFile(photo, newOutPath, prefix, rename, copy, method);
            }
        }


        private static void _organizeFile(Photo photo, string newOutPath, string prefix, bool rename, bool copy,
            OrganizeMethod method)
        {
            if (!photo.Renamable) return;

            string newPath = null;
            switch (method)
            {
                case OrganizeMethod.ByYear:
                    newPath = $"{photo.getYear()}";
                    break;
                case OrganizeMethod.ByYearMonth:
                    newPath = $"{photo.getYear()}-{photo.getMonth()}";
                    break;
                case OrganizeMethod.ByMonthInYear:
                    newPath = $"{photo.getYear()}{Path.DirectorySeparatorChar}{photo.getMonth()}";
                    break;
            }

            newOutPath += Path.DirectorySeparatorChar + newPath + Path.DirectorySeparatorChar;
            if (!Directory.Exists(newOutPath))
                Directory.CreateDirectory(newOutPath);

            if (rename)
                newOutPath += photo.getShamsiName(prefix);
            else
                newOutPath += photo.FileName;

            try
            {
                if (copy)
                    File.Copy(photo.FilePath, newOutPath);
                else File.Move(photo.FilePath, newOutPath);
            }
            catch (Exception e)
            {
                //TODO
            }
        }

        //TODO add option to delete empty dirs
        public static void processDirectory(string startLocation)
        {
            foreach (var directory in Directory.GetDirectories(startLocation))
            {
                processDirectory(directory);
                if (Directory.GetFiles(directory).Length == 0 &&
                    Directory.GetDirectories(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                }
            }
        }
    }
}