namespace SPA2.Models
{
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class Error : DbContext
    {
        public string text { get; set; }
        public Error(string t)
        {
            text = t;
        }
    }

}