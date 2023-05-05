using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace grpc.Domain.Others
{
    public class DomainException : Exception
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public DomainException(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
