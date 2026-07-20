using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CRM.Core.Common.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Designed for enums without the flag attribute. Gets the Name value of the Display attribue.
        /// </summary>
        /// <param name="enumValue">Non flagged enum.</param>
        /// <returns></returns>
        public static DisplayAttribute GetDisplay(this Enum enumValue)
        {
            if (enumValue.ToString() == "")
            {
                return new DisplayAttribute();
            }

            MemberInfo? objMemberInfo = enumValue.GetType().GetMember(enumValue.ToString()).FirstOrDefault();
            if (objMemberInfo != null)
            {
                DisplayAttribute? objAttr = objMemberInfo.GetCustomAttribute<DisplayAttribute>();
                if (objAttr != null)
                {
                    return objAttr;
                }
            }

            return new DisplayAttribute();
        }

        /// <summary>
        /// Designed for enums that sport the Flag attibute. Will get all display names of all selected values in the enum.
        /// </summary>
        /// <param name="enumValue">Flagged enum.</param>
        /// <returns></returns>
        public static IEnumerable<String> GetDisplayNames(this Enum enumValue)
        {
            List<String> colNames = new List<String>();

            if (Convert.ToInt32(enumValue) == 0)
            {
                String? strName = enumValue.GetDisplay().Name;

                if (strName != null)
                {
                    colNames.Add(strName);
                }
                return colNames;
            }

            foreach (Enum objEnm in Enum.GetValues(enumValue.GetType()))
            {
                if (Convert.ToInt32(objEnm) != 0 && enumValue.HasFlag(objEnm))
                {
                    String? strName = objEnm.GetDisplay().Name;

                    if (strName != null)
                    {
                        colNames.Add(strName);
                    }
                }
            }

            return colNames;
        }

        public static String? GetDisplayNamesForList(this Enum enumValue, String strLastSeperator = "or")
        {
            IEnumerable<String> colDisplayNames = GetDisplayNames(enumValue);
            Int32 intNumberOfItems = colDisplayNames.Count();

            if (intNumberOfItems < 2)
            {
                return colDisplayNames.FirstOrDefault();
            }
            else
            {
                String strOutput = "";
                for (Int32 intIndex = 0; intIndex < intNumberOfItems; intIndex++)
                {
                    String strDN = colDisplayNames.ElementAt(intIndex);

                    if (!String.IsNullOrEmpty(strOutput))
                    {
                        if (intIndex == intNumberOfItems - 1)
                        {
                            strOutput += " " + strLastSeperator + " ";
                        }
                        else
                        {
                            strOutput += ", ";
                        }
                    }

                    strOutput += strDN;
                }

                return strOutput;
            }
        }
    }
}
