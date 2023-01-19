using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Cardprint.Models;

namespace Cardprint;

public static class XmlReader
{
    public static List<LayoutModel> GetLayouts(string path)
    {
        List<LayoutModel> layouts = new List<LayoutModel>();

        try
        {
            var files = Directory.GetFiles(path, "*.xml");
            foreach (var file in files)
            {
                layouts.Add(GetLayoutFromXml(file));
            }

            return layouts;

        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            return layouts;
        }

    }


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


    private static  LayoutModel GetLayoutFromXml(string filePath)
    {
        XmlDocument doc = new XmlDocument();
        doc.Load(filePath);
        var layoutName = Path.GetFileNameWithoutExtension(filePath);
        var backgroundImg = doc.GetElementsByTagName("backgroundImg").Item(0)?.InnerText;
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
                }
            }
            fieldsList.Add(fieldModel);
        }
        return new LayoutModel(layoutName, backgroundImg, format, fieldsList);
    }
}
