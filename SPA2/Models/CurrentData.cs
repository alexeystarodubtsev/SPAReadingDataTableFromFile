namespace SPA2.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class CurrentData
    {
        public string filePath { get; set; }
        public string[] file { get; set; }
        public char delimeter { get; set; }
        public bool hasCaption { get; set; }
        public string caption { get; set; }
        public string errortext { get; set; }
    }


}