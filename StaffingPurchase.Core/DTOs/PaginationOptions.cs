namespace StaffingPurchase.Core
{
    public class PaginationOptions
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string Sort { get; set; }
        public string Dir { get; set; }
        public string SortExpression { get { return string.Format("{0} {1}", Sort, Dir); } }

        public static readonly PaginationOptions Default = new PaginationOptions()
        {
            PageIndex = 1,
            PageSize = 10
        };

        public static PaginationOptions GetPaginationOptions(PaginationOptions options)
        {
            return (options != null && options.PageIndex > 0 && options.PageIndex > 0) ? options : Default;
        }
    }
}
