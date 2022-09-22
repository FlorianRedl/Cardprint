﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Cardprint.Models;

namespace Cardprint;

public static class XmlReader
{
    public static List<LayoutModel> GetLayouts()
    {
        List<LayoutModel> layouts = new List<LayoutModel>();

        var files =  Directory.GetFiles(@"C:\temp\Layouts","*.xml");

        foreach (var file in files)
        {
            layouts.Add(GetLayoutFromXml(file));
        }
        
        return layouts;
        
    }




    private static  LayoutModel GetLayoutFromXml(string filePath)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(filePath);
        var ln =Path.GetFileNameWithoutExtension(filePath);
        var backgroundImg = doc.GetElementsByTagName("backgroundImg").Item(0)?.InnerText;

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
                }
            }
            fieldsList.Add(fieldModel);
        }
        return new LayoutModel(1, ln, backgroundImg, fieldsList);
    }
}