using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace TrainTickets.Enums
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue
                .GetType()
                .GetMember(enumValue.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()?
                .Name ?? enumValue.ToString();
        }

        public static string GetRoleDisplayName(this string roleName)
        {
            if (Enum.TryParse<Role>(roleName, out var roleEnum))
            {
                return roleEnum.GetDisplayName();
            }

            return roleName;
        }
    }
}
