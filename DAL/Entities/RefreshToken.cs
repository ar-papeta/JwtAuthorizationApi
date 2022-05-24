using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class RefreshToken
    {
        [ForeignKey("User")]
        public Guid Id { get; set; }
        public string? Token { get; set; }
        public DateTime ExpiryTime { get; set; }
        public User User { get; set; }
    }
}
