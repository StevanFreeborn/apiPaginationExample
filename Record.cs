using System.Collections.Generic;

namespace apiPaginationExample
{
    public class Record
    {
        public int AppId { get; set; }
        public int RecordId { get; set; }
        public List<FieldData> FieldData { get; set; }
    }
}
