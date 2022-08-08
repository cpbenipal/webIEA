using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace webIEA.Dtos
{
    public class MemberDocumentDto
    {
        public long Id { get; set; }
        public long? MemberId { get; set; }
        public string DocumentName { get; set; }
        public string ContentType { get; set; }
        public string Path { get; set; }
    }
}
