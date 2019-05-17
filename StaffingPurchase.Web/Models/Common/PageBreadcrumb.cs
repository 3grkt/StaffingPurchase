using System.Collections.Generic;

namespace StaffingPurchase.Web.Models.Common
{
    public class PageBreadcrumb : ViewModelBase
    {
        private IEnumerable<KeyValuePair<string, string>> _breadcrumb;

        public IEnumerable<KeyValuePair<string, string>> Breadcrumb
        {
            get { return _breadcrumb ?? new KeyValuePair<string, string>[] {}; }
            set { _breadcrumb = value; }
        }

        public PageBreadcrumb(string text, string url)
            : this(new[] {new KeyValuePair<string, string>(text, url)})
        {
        }

        public PageBreadcrumb(IEnumerable<KeyValuePair<string, string>> breadcrumb)
        {
            this.Breadcrumb = breadcrumb;
        }
    }
}