using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Entities
{
    public class RefreshToken
    {
        public string? Token { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public User User { get; set; }
    }
}
