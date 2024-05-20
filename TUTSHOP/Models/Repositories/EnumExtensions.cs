using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var attribute = value.GetType()
                             .GetMember(value.ToString())
                             .FirstOrDefault()
                             ?.GetCustomAttribute<DisplayAttribute>();

        return attribute?.Name ?? value.ToString();
    }
}
