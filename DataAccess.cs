using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Cardprint.Models;

namespace Cardprint;

static partial class DataAccess
{
    public static List<FieldValueAttribute> LoadFieldValueAttribute(string path, out string error)
    {
        error = string.Empty;
        List<FieldValueAttribute> list = new();
        if (!File.Exists(path))
        {
            error = $"File does not exist ({path})";
            return new List<FieldValueAttribute>();
        }

        try
        {
            using (var stream = File.OpenRead(path))
            {
                var xmlSerializer = new XmlSerializer(typeof(List<FieldValueAttribute>));
                var l = xmlSerializer.Deserialize(stream) as List<FieldValueAttribute>;
                if (l != null) list = l;
                return list;

            }

        }
        catch (Exception ex)
        {
            error = ex.Message;
            return new List<FieldValueAttribute>();
        }

    }
}
