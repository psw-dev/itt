using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;

namespace PSW.ITT.Common
{
    public static class Utility
    {
        /// <summary>
        /// Checks if incoming type inherits from Generic Type 
        /// </summary>
        /// <param name="givenType"></param>
        /// <param name="genericType"></param>
        /// <returns></returns>
        public static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            var baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }

        /// <summary>
        ///     Checks if requested user role found in list of roles in claims
        ///     if found then returns entity having role code and role id otherwise returns null
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="roleCode"></param>
        /// <returns>UserRoleDTO</returns>

        public static string DateFormatInbox(this DateTime dt)
        {
            var date = CultureInfo.CreateSpecificCulture("en-US").DateTimeFormat;
            date.ShortDatePattern = @"dd-MM-yyyy";
            return $"{dt.ToString("d", date)} - {dt.DayOfWeek.ToString().Substring(0, 3)}";
        }

        public static string GetDifferenceInHours(this DateTime dt)
        {
            return (DateTime.Now - dt).TotalHours.ToString("N0");
        }
    }
}