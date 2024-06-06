﻿using Cardprint.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cardprint.Utilities;

static class FieldValueAttributeHandler
{
    public static string CheckAndReplace(IField field)
    {
        if (field is TextFieldModel textfield)
        {
            if (string.IsNullOrEmpty(textfield.Text)) return "";

            string tempValue = textfield.Text;

            if (textfield.Text.Contains($"[date]"))
            {
            
                tempValue = tempValue.Replace("[date]", DateTime.Now.ToString("dd.MM.yyyy"));
            }
            if (textfield.Text.Contains($"[winUser]"))
            {
                var username = Environment.UserName;
                tempValue = tempValue.Replace("[winUser]", username);
            }
            return tempValue;

        }
        else
        {
            return "";
        }


    }



   
}
