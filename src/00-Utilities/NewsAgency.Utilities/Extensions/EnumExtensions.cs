using System.ComponentModel;
using System.Reflection;

namespace NewsAgency.Utilities.Extensions;
public static class EnumExtensions
{
    public static string GetEnumDescription(this Enum enumValue)
    {
        var memberInfo = enumValue.GetType()
            .GetField(enumValue.ToString());

        var descriptionAttribute = memberInfo?
            .GetCustomAttribute<DescriptionAttribute>();

        return descriptionAttribute?.Description
               ?? enumValue.ToString();
    }
}