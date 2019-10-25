using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using SPA2.Models;
using System.Web.Routing;

namespace SPA2.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {

            return View();
        }
        [HttpGet]
        public ActionResult FileNull()
        {

            return View(new Error(CurrentData.errortext));
        }
        

        [HttpPost]
        public ActionResult DocTable(Models.Settings settings)
        {
            if (settings.filePath is null)
            {
                CurrentData.errortext = "Файл не выбран";
                return RedirectToAction("FileNull");
               
                
            }
            Models.Data data = new Models.Data();
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "App_Data" + settings.filePath;
                data.file = System.IO.File.ReadAllLines(path, Encoding.Default);
                if (data.file.Count() == 0)
                {
                    CurrentData.errortext = "В выбранном файле нет записей";
                    return RedirectToAction("FileNull");
                }
                CurrentData.filePath = path;
            }
            catch (System.IO.FileNotFoundException)
            {
                CurrentData.errortext = "Такого файла не существует, выберите другой";
                return RedirectToAction("FileNull");
            }
            
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
                try
                {
                    data.Caption = data.file[0];
                    data.file = data.file.Where(m => m != data.file.First()).ToArray();
                }
                catch(IndexOutOfRangeException)
                {
                    CurrentData.errortext = "В выбранном файле нет записей";
                    return RedirectToAction("FileNull");
                    
                }

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
            return PartialView(data);
        }
        [HttpPost]
        public ActionResult updateCeil(string ceil, string row, string col)
         {
            //System.IO.File.WriteAllLines();
            try
            {
                if (col == null)
                {
                    Models.CurrentData.hasCaption = true;
                    string[] caption = Models.CurrentData.caption.Split(Models.CurrentData.delimeter);
                    caption[Convert.ToInt32(row)] = ceil;
                    Models.CurrentData.caption = String.Join(Convert.ToString(Models.CurrentData.delimeter), caption);
                    CurrentData.caption = CurrentData.caption.TrimEnd(CurrentData.delimeter);
                }
                else
                {
                    if (Models.CurrentData.file.Count() - 1 < Convert.ToInt32(row))
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
                        string[] sarr = s1.Split(CurrentData.delimeter);
                        sarr[Convert.ToInt32(col)] = ceil;
                        s1 = "";
                        for (int i = 0; i < sarr.Length; i++)
                        {
                            s1 = s1 + sarr[i];
                            if (i < sarr.Length - 1)
                                s1 = s1 + CurrentData.delimeter;
                        }

                        CurrentData.file[Convert.ToInt32(row)] = s1;
                    }
                }
            }
            catch(Exception)
            {
                CurrentData.errortext = "Что-то пошло не так.."; //не должны сюда попасть, но вдруг
                return RedirectToAction("FileNull");
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
                
                //return PartialView(new Models.Ceil());
                //иногда летит ошибка, что файл занят другим процессом
            }
            Models.Ceil Ceil = new Models.Ceil();
            Ceil.value = ceil;
            return PartialView(Ceil);
        }
        public ActionResult Directory(string dir, string curDir)
       {
            return ChooseFile(dir, curDir);
        }
        public ActionResult ChooseFile(string dir, string curDir)
        {

            ListFiles lf = new ListFiles();
            
            lf.curDirectory = AppDomain.CurrentDomain.BaseDirectory + "App_Data";
            lf.prevdir = "";
            if (curDir != null && dir!=null)
            {
                lf.curDirectory = lf.curDirectory + curDir + dir;
                lf.prevdir = curDir;
            }
            try
            {
                lf.files = System.IO.Directory.GetFiles(lf.curDirectory);
                lf.directories = System.IO.Directory.GetDirectories(lf.curDirectory);
            }
            catch(System.IO.DirectoryNotFoundException)
            {
                CurrentData.errortext = "Невозможно получить доступ к папке, попробуйте снова"; //для случаев, когда папка была удалена после получения списка
                return RedirectToAction("FileNull");
            }
            
            for (int i = 0; i< lf.files.Count(); i++)
            {
                lf.files[i] = lf.files[i].Replace(lf.curDirectory, "");
            }
            for (int i = 0; i < lf.directories.Count(); i++)
            {
                lf.directories[i] = lf.directories[i].Replace(lf.curDirectory, "");
            }
            lf.curDirectory = lf.curDirectory.Replace(AppDomain.CurrentDomain.BaseDirectory + "App_Data", "");
            
            return PartialView(lf);
            //return HttpNotFound();
        } 
         
    

    }
}