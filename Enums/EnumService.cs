using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace TrainTickets.Enums
{
    public class EnumService<T> : IEnumService<T> where T : Enum
    {
        public IEnumerable<EnumDTO> GetAll()
        {
            IEnumerable<T> enumValues = Enum.GetValues(typeof(T)) as IEnumerable<T>;

            IEnumerable<EnumDTO> enumDTOs = enumValues.Select(enumValue => new EnumDTO
            {
                Value = Convert.ToInt32(enumValue),
                Text = GetDisplayName(enumValue)
            });

            return enumDTOs;
        }

        private string GetDisplayName(T enumValue)
        {
            MemberInfo memberInfo = typeof(T).GetMember(enumValue.ToString()).FirstOrDefault();
            
            DisplayAttribute displayAttribute = memberInfo
                .GetCustomAttributes(typeof(DisplayAttribute))
                .FirstOrDefault() as DisplayAttribute;

            return displayAttribute.Name;
        }
    }
}
