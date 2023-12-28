namespace Net8CoreApiBoilerplate.Services.Shared
{
    public class BaseGridFilter
    {
        public int PageSize { get; set; }
        public int PageIndex { get; set; } // Zero Based
        public string SortFieldName { get; set; }
        public string SortDirection { get; set; }
        public int Skip => PageIndex * PageSize;
        public int Take => PageSize;
    }
}
