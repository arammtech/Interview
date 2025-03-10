using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interview.Domain.Common;
using Interview.Domain.Identity;

namespace Interview.Domain.Entities
{
    public class Review : BaseEntity
    {
        public int UserId { get; set; }
        public string Comment { get; set; } = null!;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
