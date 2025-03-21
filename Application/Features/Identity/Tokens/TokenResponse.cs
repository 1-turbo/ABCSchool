using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Identity.Tokens
{
    public class TokenResponse
    {
        public string Jwt { get; set; }
        public string RefreshToken { get; set; }
        public  DateTime RefreshTokenExpiryDate { get; set; }
    }
}
