namespace SPA2.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public  class Settings 
    {
        public string DBName { get; set; }
        
        public string filePath { get; set; }
        public bool hasCaption { get; set; }
        public Delimeter delim { get; set; }
        public char otherDelim { get; set; }
        

    }


}