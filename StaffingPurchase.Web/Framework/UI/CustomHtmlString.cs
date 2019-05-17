using System.Web;

namespace StaffingPurchase.Web.Framework.UI
{
    public class CustomHtmlString : IHtmlString
    {
        private readonly string _text;

        public CustomHtmlString(string text)
        {
            _text = text;
        }

        /// <summary>
        /// Gets HTML-encoded string.
        /// </summary>
        /// <returns></returns>
        public string ToHtmlString()
        {
            return _text;
        }

        /// <summary>
        /// Gets string used in javascript expression.
        /// </summary>
        /// <remarks>
        /// Quote (') or double quote (") character will replaced to avoid javascript error.
        /// </remarks>
        /// <param name="useDoubleQuote">true: if use double quote (") in javascript expression; false: if use quote (').</param>
        /// <returns></returns>
        public IHtmlString ToJavascriptString(bool useDoubleQuote = false)
        {
            if (useDoubleQuote)
                return new HtmlString(_text.Replace("\"", "\\\""));
            return new HtmlString(_text.Replace("'", "\\'"));
        }

        public override string ToString()
        {
            return _text.ToString();
        }

        /// <summary>
        /// Casts CustomHtml object to string.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static implicit operator string(CustomHtmlString obj)
        {
            return obj._text;
        }
    }
}