using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Common;
using Entities.User;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Services.Services
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JwtService(IOptionsSnapshot<SiteSettings> siteSettings)
        {
            _jwtSettings = siteSettings.Value.JwtSettings;
        }
        public string Generate(User user)
        {
            var securityKey = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(securityKey)
                , SecurityAlgorithms.HmacSha256Signature);
            var encryptKey=Encoding.UTF8.GetBytes(_jwtSettings.EncryptKey);
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encryptKey),
                SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);
            var descriptor = new SecurityTokenDescriptor()
            {
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                IssuedAt = DateTime.Now,
                NotBefore = DateTime.Now.AddMinutes(_jwtSettings.NotBeforeMinutes),
                Expires = DateTime.Now.AddDays(_jwtSettings.ExpirationDays),
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials,
                Subject = new ClaimsIdentity(_getClaims(user))
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(descriptor);
            var jwt = tokenHandler.WriteToken(securityToken);
            return jwt;
        }

        private IEnumerable<Claim> _getClaims(User user)
        {
            var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(ClaimTypes.MobilePhone,"09222192282"),
                    new Claim("Age",user.Age.ToString())
                };
            return claims;
        }
    }
}
