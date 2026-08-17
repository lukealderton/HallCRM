using CRM.Primitives.DataAnnotations;
using System.Reflection;

namespace CRM.Primitives.Extensions
{
    public static class UIExtensions
    {
        /// <summary>
        /// Designed for enums without the flag attribute. Gets the Name value of the Display attribue.
        /// </summary>
        /// <param name="enumValue">Non flagged enum.</param>
        /// <returns></returns>
        public static UIAttribute GetUI(this Enum enumValue)
        {
            if (enumValue.ToString() == "")
            {
                return new UIAttribute();
            }

            MemberInfo? objMemberInfo = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();
            if (objMemberInfo != null)
            {
                UIAttribute? objAttr = objMemberInfo.GetCustomAttribute<UIAttribute>();
                if (objAttr != null)
                {
                    return objAttr;
                }
            }

            return new UIAttribute();
        }
    }
}
