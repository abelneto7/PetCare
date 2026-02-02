using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;

namespace PetCare.Web.Helpers
{
    public static class EnumDisplayHelper
    {
        public static string ToDisplay(this Enum value)
        {
            var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            var display = member?.GetCustomAttribute<DisplayAttribute>();
            if (!string.IsNullOrWhiteSpace(display?.Name))
                return display!.Name!;

            return HumanizeEnumName(value.ToString());
        }

        public static string HumanizeEnumName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;

            var sb = new StringBuilder();
            sb.Append(name[0]);

            for (int i = 1; i < name.Length; i++)
            {
                char c = name[i];
                char prev = name[i - 1];

                if (char.IsUpper(c) && (char.IsLower(prev) || char.IsDigit(prev)))
                    sb.Append(' ');

                sb.Append(c);
            }

            var s = sb.ToString();
            s = s.Replace(" Da ", " da ")
                 .Replace(" De ", " de ")
                 .Replace(" Do ", " do ")
                 .Replace(" Das ", " das ")
                 .Replace(" Dos ", " dos ");

            return s.Trim();
        }
    }
}
