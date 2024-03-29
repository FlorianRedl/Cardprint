﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Cardprint.Models;


namespace Cardprint;

public static class DataAccess
{
    public static List<string> GetLayoutNames(string path)
    {
        var layoutNames = new List<string>();
        try
        {
            var files = Directory.GetFiles(path, "*.xml");
            foreach (var file in files)
            {
                layoutNames.Add(Path.GetFileNameWithoutExtension(file));
            }
            return layoutNames;

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return layoutNames;
        }
    }

    public static LayoutModel GetLayout(string filePath,string layoutName)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(filePath+"\\"+layoutName+".xml");
        //var backgroundImg = doc.GetElementsByTagName("backgroundImg").Item(0)?.InnerText;
        var format = doc.GetElementsByTagName("format").Item(0)?.InnerText;
        var fields = doc.GetElementsByTagName("field");
        List<FieldModel> fieldsList = new List<FieldModel>();
        foreach (XmlNode field in fields)
        {
            FieldModel fieldModel = new();
            foreach (XmlNode item in field.ChildNodes)
            {
                switch (item.Name)
                {
                    case "name":
                        fieldModel.Name = item.InnerText;
                        break;
                    case "x":
                        fieldModel.XCord = double.Parse(item.InnerText);
                        break;
                    case "y":
                        fieldModel.YCord = double.Parse(item.InnerText);
                        break;
                    case "size":
                        fieldModel.Size = double.Parse(item.InnerText);
                        break;
                    case "value":
                        fieldModel.Value = item.InnerText;
                        break;
                }
            }
            fieldsList.Add(fieldModel);
        }
        return new LayoutModel(layoutName, format, fieldsList);
    }
}
