using System;

namespace Cardprint.Models;

public class FieldValueAttribute
{
    public string Key { get; set; }
    public string Value { get; set; }
    public FieldValueAttribute()
    {
    }

    public FieldValueAttribute(string key, string value)
    {
        Key = key;
        Value = value;
    }
}

