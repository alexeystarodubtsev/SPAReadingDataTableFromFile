﻿using System;
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
        CurrentData currentData;
        [HttpGet]
        public ActionResult Index()
        {

            return View();
        }
        [HttpGet]
        public ActionResult FileNull()
        {
            try
            {
                currentData = (CurrentData)Session["currentData"];
            }
            catch
            {
                return View(new Error("Неопознанная ошибка"));
            }
            return View(new Error(currentData.errortext));
            
        }
        

        [HttpPost]
        public ActionResult DocTable(Models.Settings settings)
        {
            currentData = new CurrentData();
            
            
            if (settings.filePath is null)
            {
                currentData.errortext = "Файл не выбран";
                Session["currentData"] = currentData;
                return RedirectToAction("FileNull");
               
                
            }
            Models.Data data = new Models.Data();
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "App_Data" + settings.filePath;
                data.file = System.IO.File.ReadAllLines(path, Encoding.Default);
                if (data.file.Count() == 0)
                {
                    currentData.errortext = "В выбранном файле нет записей";
                    Session["currentData"] = currentData;
                    return RedirectToAction("FileNull");
                }
                currentData.filePath = path;
            }
            catch (System.IO.FileNotFoundException)
            {
                
                currentData.errortext = "Такого файла не существует, выберите другой";
                Session["currentData"] = currentData;
                return RedirectToAction("FileNull");
            }
            catch (System.IO.IOException)
            {

                currentData.errortext = "Данный файл занят другим процессом";
                Session["currentData"] = currentData;
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
                    currentData.errortext = "В выбранном файле нет записей";
                    Session["currentData"] = currentData;
                    return RedirectToAction("FileNull");
                    
                }

            }
            else
                for (int i = 1; i <= data.colsNum; i++)
                {
                    data.Caption = data.Caption + "col_" + i + data.charDelimeter;
                        

                }
            data.Caption = data.Caption.TrimEnd(data.charDelimeter);
            currentData.file = data.file;
                
            currentData.hasCaption = settings.hasCaption;
            currentData.delimeter = data.charDelimeter;
            currentData.caption = data.Caption;
            Session["currentData"] = currentData;
            return PartialView(data);
        }
        [HttpPost]
        public ActionResult updateCeil(string ceil, string row, string col)
         {
            try
            {
                currentData = (CurrentData)Session["currentData"];
            }
            catch (Exception)
            {
                RedirectToAction("FileNull");
            }
            //System.IO.File.WriteAllLines();
            try
            {
                if (col == null)
                {
                    currentData.hasCaption = true;
                    string[] caption = currentData.caption.Split(currentData.delimeter);
                    caption[Convert.ToInt32(row)] = ceil;
                    currentData.caption = String.Join(Convert.ToString(currentData.delimeter), caption);
                    currentData.caption = currentData.caption.TrimEnd(currentData.delimeter);
                }
                else
                {
                    if (currentData.file.Count() - 1 < Convert.ToInt32(row))
                    {
                        List<string> ls = currentData.file.ToList();
                        string s1 = "";
                        for (int i = 0; i < currentData.caption.Trim(currentData.delimeter).Split(currentData.delimeter).Count(); i++)
                        {
                            if (i == Convert.ToInt32(col))
                                s1 = ceil;
                            if (i < currentData.caption.Trim(currentData.delimeter).Split(currentData.delimeter).Count() - 1)
                                s1 += currentData.delimeter;
                        }
                        ls.Add(s1);
                        currentData.file = ls.ToArray();
                    }
                    else
                    {
                        string s1 = currentData.file[Convert.ToInt32(row)];
                        string[] sarr = s1.Split(currentData.delimeter);
                        sarr[Convert.ToInt32(col)] = ceil;
                        s1 = "";
                        for (int i = 0; i < sarr.Length; i++)
                        {
                            s1 = s1 + sarr[i];
                            if (i < sarr.Length - 1)
                                s1 = s1 + currentData.delimeter;
                        }

                        currentData.file[Convert.ToInt32(row)] = s1;
                    }
                }
            }
            catch(Exception)
            {
                if (currentData != null)
                {
                    currentData.errortext = "Что-то пошло не так.."; //не должны сюда попасть, но вдруг
                    Session["currentData"] = currentData;
                }
                return RedirectToAction("FileNull");
            }
            List<string> DataForFIle = new List<string>();
            if (currentData.hasCaption)
                DataForFIle.Add(currentData.caption);
            DataForFIle.AddRange(currentData.file);
            try
            {
                System.IO.File.WriteAllLines(currentData.filePath, DataForFIle, Encoding.Default);
            }
            catch(Exception)
            {
                
                //return PartialView(new Models.Ceil());
                //иногда летит ошибка, что файл занят другим процессом
            }
            Models.Ceil Ceil = new Models.Ceil();
            Ceil.value = ceil;
            Session["currentData"] = currentData;
            return PartialView(Ceil);
        }
        public ActionResult Directory(string dir, string curDir)
       {
            return ChooseFile(dir, curDir);
        }
        public ActionResult ChooseFile(string dir, string curDir)
        {
            try
            {
                currentData = (CurrentData)Session["currentData"];
            }
            catch (Exception)
            {
                RedirectToAction("FileNull");
            }

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
                currentData.errortext = "Невозможно получить доступ к папке, попробуйте снова"; //для случаев, когда папка была удалена после получения списка
                Session["currentData"] = currentData;
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

            Session["currentData"] = currentData;
            return PartialView(lf);

            //return HttpNotFound();
        } 
         
    

    }
}