namespace SPA2.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class CurrentData
    {
        public static string filePath { get; set; }
        public static string[] file { get; set; }
        public static char delimeter { get; set; }
        public static bool hasCaption { get; set; }
        public static string caption { get; set; }
        public static string errortext { get; set; }
    }


}