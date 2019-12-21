using System;
using System.Globalization;
using System.IO;

namespace Shamsi_Photo_Organizer.Model
{
    class Photo
    {
        public string FilePath{ get; }
        public string FileName { get; }
        private string Extension { get; }

        public Photo(string filePath)
        {
            FilePath = filePath;
            FileName = Path.GetFileName(filePath);
            Extension = Path.GetExtension(filePath);
        }

        public string DateTimeString { get; set; }

        public DateTime DateTime { get; set; }

        public bool Renamable { get; set; }

        private readonly PersianCalendar _persianCalendar = new PersianCalendar();

        public string getYear()
        {
            return _persianCalendar.GetYear(DateTime).ToString();
        }

        public string getMonth()
        {
            return _persianCalendar.GetMonth(DateTime).ToString();
        }

        public string getShamsiName(string prefix)
        {
            string day = _persianCalendar.GetDayOfMonth(DateTime).ToString();
            string hour = _persianCalendar.GetHour(DateTime).ToString();
            string minute = _persianCalendar.GetMinute(DateTime).ToString();
            string second = _persianCalendar.GetSecond(DateTime).ToString();
            return $"{prefix}_{getYear()}-{getMonth()}-{day}_{hour}-{minute}-{second}{Extension}";
        }
    }
}