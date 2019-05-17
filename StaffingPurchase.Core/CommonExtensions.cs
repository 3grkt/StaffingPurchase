using StaffingPurchase.Core.Infrastructure;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;

namespace StaffingPurchase.Core
{
    public static class CommonExtensions
    {
        #region IQueryable
        /// <summary>
        /// Sorts IQueryable object using sort expression.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IQueryable<T> SortBy<T>(this IQueryable<T> source, string propertyName)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            // DataSource control passes the sort parameter with a direction
            // if the direction is descending          
            string[] arr = propertyName.Split(' ');
            string sortField = arr[0];
            string sorDir = arr[1];

            if (string.IsNullOrEmpty(sortField))
            {
                return source;
            }

            string[] fields = sortField.Split('.');

            ParameterExpression parameter = Expression.Parameter(source.ElementType, string.Empty);
            MemberExpression property = Expression.Property(parameter, fields[0]);
            // there is at least 1 item in arrays => no out of range exception
            if (fields.Length > 1)
            {
                for (int i = 1; i < fields.Length; i++)
                {
                    property = Expression.Property(property, fields[i]);
                }
            }
            LambdaExpression lambda = Expression.Lambda(property, parameter);
            string methodName = sorDir.Equals("ASC", StringComparison.OrdinalIgnoreCase)
                ? "OrderBy"
                : "OrderByDescending";
            Expression methodCallExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { source.ElementType, property.Type },
                source.Expression,
                Expression.Quote(lambda));

            return source.Provider.CreateQuery<T>(methodCallExpression);
        }
        #endregion

        #region DateTime
        /// <summary>
        /// Gets end of date point.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime EndOfDate(this DateTime dt)
        {
            return CommonHelper.GetEndOfDate(dt);
        }

        /// <summary>
        /// Gets start of date point.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime StartOfDate(this DateTime dt)
        {
            return CommonHelper.GetStartOfDate(dt);
        }

        /// <summary>
        /// Returns date string in preconfigured format - e.g. 14/12/2016
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToLocalizedDate(this DateTime dt)
        {
            string dateFormat = EngineContext.Current.Resolve<IWebHelper>().DateFormat;
            return dt.ToString(dateFormat);
        }

        /// <summary>
        /// Returns date string in preconfigured format - e.g. 14/12/2016
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToLocalizedDate(this DateTime? dt)
        {
            if (dt == null)
            {
                return string.Empty;
            }

            return ToLocalizedDate(dt.Value);
        }
        #endregion

        #region String
        /// <summary>
        /// Trims string to get only {numOfWords} characters.
        /// </summary>
        /// <param name="s"></param>
        /// <param name="numOfWords"></param>
        /// <param name="getFullWord"></param>
        /// <returns></returns>
        public static string Trim(this string s, int numOfWords, bool getFullWord = false)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            if (s.Length <= numOfWords)
                return s;

            if (getFullWord)
            {
                var lastSpace = s.LastIndexOf(' ', numOfWords);
                if (lastSpace >= 0)
                    return s.Substring(0, lastSpace) + "...";
            }

            return s.Substring(0, numOfWords) + "...";
        }
        #endregion

        #region Numeric
        public static string ToCurrencyString(this decimal value)
        {
            return value.ToString(EngineContext.Current.Resolve<IWebHelper>().CurrencyFormat);
        }
        #endregion

        #region IpAddress

        public static int CompareTo(this IPAddress ipAddress, IPAddress toCompare)
        {
            var bytes1 = ipAddress.GetAddressBytes();
            var bytes2 = toCompare.GetAddressBytes();

            for (int i = 0; i < bytes1.Length; i++)
            {
                if (bytes1[i] > bytes2[i])
                {
                    return 1;
                }
                else if (bytes1[i] < bytes2[i])
                {
                    return -1;
                }
            }
            return 0;
        }

        public static bool GreaterThanOrEqual(this IPAddress ipAddress, IPAddress toCompare)
        {
            return ipAddress.CompareTo(toCompare) >= 0;
        }

        public static bool LessThanOrEqual(this IPAddress ipAddress, IPAddress toCompare)
        {
            return ipAddress.CompareTo(toCompare) <= 0;
        }

        #endregion
    }
}
