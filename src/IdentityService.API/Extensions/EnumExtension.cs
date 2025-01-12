using System.ComponentModel;

namespace IdentityService.API.Extensions
{
    public static class EnumExtension
    {
        public static string GetDescription<T>(this T value) where T : struct, IConvertible
        {
            var description = value.ToString();
            var fieldInfo = value.GetType().GetField(description!);

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                    description = ((DescriptionAttribute)attrs[0]).Description;
            }
            return description ?? string.Empty;
        }
    }
}
