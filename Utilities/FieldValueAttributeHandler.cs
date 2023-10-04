using Cardprint.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cardprint.Utilities;

static class FieldValueAttributeHandler
{
    public static string CheckAndReplace(FieldModel field)
    {
        string tempValue = field.Value;
        if (string.IsNullOrEmpty(field.Value)) return tempValue;

        if (field.Value.Contains($"[date]"))
        {
            
            tempValue = tempValue.Replace("[date]", DateTime.Now.ToString("dd.MM.yyyy"));
        }
        if (field.Value.Contains($"[winUser]"))
        {
            var username = Environment.UserName;
            tempValue = tempValue.Replace("[winUser]", username);
        }
        return tempValue;
    }



   
}
