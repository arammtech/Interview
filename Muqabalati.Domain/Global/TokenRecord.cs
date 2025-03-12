using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Muqabalati.Domain.Global
{
    public class TokenRecord
    {
        public string Token { get; set; }
        public string IPAddress { get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool Used { get; set; }
    }
}
