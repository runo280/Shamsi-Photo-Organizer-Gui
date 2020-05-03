using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Shamsi_Photo_Organizer.Model
{
    internal class Photo
    {
        public string FullPath { get; }
        public string FileDir { get; }
        public string FileName { get; }
        private string Extension { get; }

        public Photo(string filePath)
        {
            Debug.WriteLine($"file: {filePath}");
            FullPath = filePath;
            FileDir = Path.GetDirectoryName(filePath);
            FileName = Path.GetFileName(filePath);
            Extension = Path.GetExtension(filePath);
        }

        public string DateTimeString { get; set; }

        public DateTime DateTime { get; set; }

        public bool Renamable { get; set; }

        private readonly PersianCalendar _persianCalendar = new PersianCalendar();

        public string GetNewPath(string prefix)
        {
            return FileDir + Path.DirectorySeparatorChar + GetShamsiName(prefix);
        }

        public string GetYear() => _persianCalendar.GetYear(DateTime).ToString();

        public string GetMonth() => _persianCalendar.GetMonth(DateTime).ToString();

        private string GetShamsiName(string prefix)
        {
            string day = _persianCalendar.GetDayOfMonth(DateTime).ToString();
            string hour = _persianCalendar.GetHour(DateTime).ToString();
            string minute = _persianCalendar.GetMinute(DateTime).ToString();
            string second = _persianCalendar.GetSecond(DateTime).ToString();
            return $"{prefix}_{GetYear()}-{GetMonth()}-{day}_{hour}-{minute}-{second}{Extension}";
        }
    }
}