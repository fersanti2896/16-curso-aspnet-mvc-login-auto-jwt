using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Transporte;

namespace Pedidos.Utils {
    public class JwtConfigurator {
        public static object GetToken(UsuarioDTO usuario, IConfiguration config) {
            string secretKey = config["Jwt:SecretKey"];
            string issuer    = config["Jwt:Issuer"];
            string audience  = config["Jwt:Audience"];

            var secutiryKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(secutiryKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim("UsuarioId", "" + usuario.UsuarioId),
                new Claim("CatPerfilId", "" + usuario.CatPerfilId),
                new Claim("CatRegionId", "" + (usuario.CatRegionId == null ? 0 : usuario.CatRegionId)),
                new Claim("CatEstadoRepublicaId", "" + (usuario.CatEstadoRepublicaId == null ? 0 : usuario.CatEstadoRepublicaId)),
                new Claim(ClaimTypes.Role, "" + usuario.Perfil),
            };

            /* Se arma token */
            var token = new JwtSecurityToken(
                            issuer: issuer,
                            audience: audience,
                            claims, 
                            expires: DateTime.Now.AddMinutes(120),
                            signingCredentials: credentials
                        );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        internal static int GetToken(ClaimsIdentity identity, string keyClaim) {
            if (identity != null) {
                IEnumerable<Claim> Claims = identity.Claims;

                foreach (var claim in Claims) {
                    if (claim.Type == keyClaim) { 
                        return int.Parse(claim.Value);  
                    }
                }
            }

            return 0;
        }
    }
}
