using System.Collections.Generic;

namespace apiPaginationExample
{
    public class ReportResult
    {
        public List<string> Columns { get; set; }
        public List<List<string>> Rows { get; set; }
    }
}
