using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Cardprint.Models;
using static System.Net.Mime.MediaTypeNames;


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

    public static LayoutModel? GetLayout(string filePath,string layoutName,out string error)
    {
        try
        {
            error = string.Empty;
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath+"\\"+layoutName+".xml");
            var format = doc.GetElementsByTagName("format").Item(0)?.InnerText;
            var texts = doc.GetElementsByTagName("text");
            List<IField> fields = new();

            foreach (XmlNode field in texts)
            {
                var name = field.SelectSingleNode("name")?.InnerText;
                if (name is null) continue;
                var x = field.SelectSingleNode("x")?.InnerText;
                var xCord = x is null ? 0 : double.Parse(x);

                var y = field.SelectSingleNode("y")?.InnerText;
                var yCord = y is null ? 0 : double.Parse(y);

                var s = field.SelectSingleNode("size")?.InnerText;
                var size = s is null ? 0 : double.Parse(s);

                var value = field.SelectSingleNode("value")?.InnerText;

                fields.Add(new TextFieldModel(name,xCord,yCord,size,value));
            }

            var images = doc.GetElementsByTagName("image");
            foreach (XmlNode image in images)
            {
                var name = image.SelectSingleNode("name")?.InnerText;
                if (name is null) continue; //Meldung
                var x = image.SelectSingleNode("x")?.InnerText;
                var xCord = x is null ? 0 : double.Parse(x);

                var y = image.SelectSingleNode("y")?.InnerText;
                var yCord = y is null ? 0 : double.Parse(y);

                var w = image.SelectSingleNode("width")?.InnerText;
                var width = w is null ? 0 : double.Parse(w);

                var h = image.SelectSingleNode("height")?.InnerText;
                var height = h is null ? 0 : double.Parse(h);

                var path = image.SelectSingleNode("path")?.InnerText;
                if (path is null) continue; // meldung
                if(!File.Exists(path)) continue; //meldung

                fields.Add(new ImageFieldModel(name, xCord, yCord, width, height, path));
            }

            return new LayoutModel(layoutName, format, fields);

        }
        catch (Exception ex)
        {
            error = ex.Message; 
            return null;
            
        }
    }
}
