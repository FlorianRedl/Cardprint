using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
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


    public static Layout? LoadLayout(string dirpath, string layoutName, out string error)
    {
        error = string.Empty;
        var path = Path.Combine(dirpath, layoutName + ".xml");
        if (!File.Exists(path))
        {
            error = $"File does not exist ({path})";
            return null;
        }

        try
        {
            using (var stream = File.OpenRead(path))
            {
                var xmlSerializer = new XmlSerializer(typeof(Layout));
                var layoutfile = xmlSerializer.Deserialize(stream) as Layout;
                if(layoutfile is null)
                {
                    error = "Deserialization failed";
                    return null;
                }
                layoutfile.Name = layoutName;
                return layoutfile;
            }
        }
        catch (Exception ex)
        {
            error = ex.Message;
            return null;

        }


           
    }
}
