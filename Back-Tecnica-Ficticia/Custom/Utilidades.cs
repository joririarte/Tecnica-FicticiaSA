using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Tecnica_FicticiaSA.Models;

namespace Tecnica_FicticiaSA.Custom
{
    public class Utilidades
    {
        private readonly IConfiguration _config;
        public Utilidades(IConfiguration config)
        {
            _config = config;
        }

        public string encriptSHA256(string text)
        {
            using (SHA256 sha256Hash = SHA256.Create()) {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(text));

                StringBuilder strBuilder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    strBuilder.Append(bytes[i].ToString("x2"));
                }
                return strBuilder.ToString();
            }
        }

        public string createJWT(Usuario Modelo)
        {
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, Modelo.UsuarioId.ToString()),
                new Claim(ClaimTypes.Email, Modelo.UsuarioCorreo.ToString())
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var jwtConfig = new JwtSecurityToken(
                claims: userClaims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
        }
    }
}
