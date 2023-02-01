namespace PSW.ITT.Common.Pagination
{
    public class ServerPaginationModel
    {
        public int offset { get; set; }
        public int Size { get; set; }
        public string SortColumn { get; set; }
        public string SortOrder { get; set; }

    }
}