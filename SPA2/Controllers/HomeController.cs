using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using SPA2.Models;
namespace SPA2.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult FileNull()
        {

            return View();
        }
        

        [HttpPost]
        public ActionResult DocTable(Models.Settings settings)
        {
            if (settings.filePath is null)
            {
                return RedirectToAction("FileNull");
            }
            Models.Data data = new Models.Data();
            //C:/Users/xiaomi/Desktop/f_tn_table_supplier.txt
            try
            {
                data.file = System.IO.File.ReadAllLines(settings.filePath, Encoding.Default);
             }
            catch (Exception e)
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "App_Data\\f_tn_table_supplier.txt";
                data.file = System.IO.File.ReadAllLines(path, Encoding.Default);
                Models.CurrentData.filePath = path;
                

            }
            finally
            {
                switch (settings.delim)
                {
                    case Models.Delimeter.space:
                        data.charDelimeter = ' ';
                        break;
                    case Models.Delimeter.semicolon:
                        data.charDelimeter = ';';
                        break;
                    case Models.Delimeter.other:
                        data.charDelimeter = settings.otherDelim;
                        break;
                    default:
                        data.charDelimeter = '\t';
                        break;
                }
                

                data.colsNum = 0;
                foreach (string line in data.file)
                {
                    int linecount = line.Split(data.charDelimeter).Count();
                    if (data.colsNum < linecount)
                        data.colsNum = linecount;
                }
                data.Caption = "";
                if (settings.hasCaption)
                {
                    data.Caption = data.file[0];
                    data.file = data.file.Where(m => m != data.file.First()).ToArray();
                    
                }
                else
                    for (int i = 1; i <= data.colsNum; i++)
                    {
                        data.Caption = data.Caption + "col_" + i + data.charDelimeter;
                        

                    }
                data.Caption = data.Caption.TrimEnd(data.charDelimeter);
                Models.CurrentData.file = data.file;
                
                Models.CurrentData.hasCaption = settings.hasCaption;
                Models.CurrentData.delimeter = data.charDelimeter;
                Models.CurrentData.caption = data.Caption;
            }
            return PartialView(data);
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult updateCeil(string ceil, string row, string col)
         {
            //System.IO.File.WriteAllLines();
            if (col == null)
            {
                Models.CurrentData.hasCaption = true;
                string[] caption = Models.CurrentData.caption.Split(Models.CurrentData.delimeter);
                caption[Convert.ToInt32(row)] = ceil;
                Models.CurrentData.caption = String.Join(Convert.ToString(Models.CurrentData.delimeter),caption);
                CurrentData.caption = CurrentData.caption.TrimEnd(CurrentData.delimeter);
            }
            else
            {   if (Models.CurrentData.file.Count() - 1 < Convert.ToInt32(row))
                {
                    List<string> ls = CurrentData.file.ToList();
                    string s1 = "";
                    for (int i = 0; i < CurrentData.caption.Trim(CurrentData.delimeter).Split(CurrentData.delimeter).Count(); i++)
                    {
                        if (i == Convert.ToInt32(col))
                            s1 = ceil;
                        if (i < CurrentData.caption.Trim(CurrentData.delimeter).Split(CurrentData.delimeter).Count() - 1)
                          s1 += CurrentData.delimeter;
                    }
                    ls.Add(s1);
                    CurrentData.file = ls.ToArray();
                }
                else
                {
                    string s1 = CurrentData.file[Convert.ToInt32(row)];
                    string [] sarr = s1.Split(CurrentData.delimeter);
                    sarr[Convert.ToInt32(col)] = ceil;
                    s1="";
                    for (int i = 0; i < sarr.Length; i++)
                    {
                        s1 = s1 + sarr[i];
                        if (i < sarr.Length - 1)
                            s1 = s1 + CurrentData.delimeter;
                    }
                   
                    CurrentData.file[Convert.ToInt32(row)] = s1;
                }
            }
            List<string> DataForFIle = new List<string>();
            if (CurrentData.hasCaption)
                DataForFIle.Add(CurrentData.caption);
            DataForFIle.AddRange(CurrentData.file);
            try
            {
                System.IO.File.WriteAllLines(CurrentData.filePath, DataForFIle, Encoding.Default);
            }
            catch(Exception)
            {

            }
            Models.Ceil Ceil = new Models.Ceil();
            Ceil.value = ceil;
            return PartialView(Ceil);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}