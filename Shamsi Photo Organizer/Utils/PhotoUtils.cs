using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Shamsi_Photo_Organizer.Model;

namespace Shamsi_Photo_Organizer.Utils
{
    internal static class PhotoUtils
    {
        private static readonly string[] DateTimeFormats = {"yyyy:MM:dd HH:mm:ss"};

        public static List<PhotoItem> GetPhotosList(string dir) => FileUtils.GetPhotosListAsString(dir).Select(file =>
        {
            var photo = new PhotoItem(file);
            var dateTimeString = ExifUtils.FindDateTimeFromFile(file);
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

        public static void RenamePhotos(List<PhotoItem> photos, string prefix)
        {
            foreach (var photo in photos)
            {
                FileUtils.Rename(photo, prefix);
            }
        }

        public static void OrganizePhotos(List<PhotoItem> photos, OrganizeMethod method)
        {
            foreach (var photo in photos)
            {
                FileUtils.Organize(photo, method);
            }
        }
    }
}