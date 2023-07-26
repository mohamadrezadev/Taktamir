using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Taktamir.Core.Domain._03.Users;

namespace Taktamir.Services.JwtServices
{
    public interface IJwtService
    {
        JwtSecurityToken GenerateRefreshToken(string Access_Token);

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

        bool ValidateToken(string token);

        JwtSecurityToken CreateToken(User user);

        string DecodeRefreshToken(string Token);
    
    }
    public class JwtService : IJwtService
    {
        private readonly IConfiguration iconfiguration;
        private SymmetricSecurityKey _authSigningKey;
        private SigningCredentials _signingCredentials;

        const string TokenValidityInMinutesConfigKey = "JWT:TokenValidityInMinutes";
        const string SecretConfigKey = "JWT:Secret";
        const string ValidIssuerConfigKey = "JWT:ValidIssuer";
        const string ValidAudienceConfigKey = "JWT:ValidAudience";

        public JwtService(IConfiguration iconfiguration)
        {
            this.iconfiguration = iconfiguration;
        }

      
        public JwtSecurityToken CreateToken(User user)
        {
            List<Claim> claims = new List<Claim> {
                new Claim("MobilePhone", user.PhoneNumber),
                new Claim("Name", user.UserName),
               
            };
            if (_authSigningKey == null)
            {
                _authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(iconfiguration[SecretConfigKey]));
            }

            if (_signingCredentials == null)
            {
                _signingCredentials = new SigningCredentials(_authSigningKey, SecurityAlgorithms.HmacSha256);
            }

            var tokenValidityInMinutes = int.TryParse(iconfiguration[TokenValidityInMinutesConfigKey], out int minutes) ? minutes : 60;
            var token = new JwtSecurityToken(
                issuer: iconfiguration[ValidIssuerConfigKey],
                audience: iconfiguration[ValidAudienceConfigKey],
                expires: DateTime.Now.AddDays(15),
                claims: claims,
                signingCredentials: _signingCredentials
            );
            return token;
        }

        public JwtSecurityToken GenerateRefreshToken(string Userid)
        {
            if (Userid == null)
            {
                throw new ArgumentException("Access token should not be null", nameof(Userid));
            }

            // Generate a new random number for the refresh token
            var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            // Create a new Claim for the access token
            var useridClaim = new Claim("Userid", Userid);

            // Create a new JwtSecurityToken for the refresh token
            var refreshTokenSecurityToken = new JwtSecurityToken(
                refreshToken,
                claims: new[] { useridClaim },
                expires: DateTime.UtcNow.AddDays(365), // set an expiration date
                signingCredentials: GetSigningCredentials()
            );

            return refreshTokenSecurityToken;

        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var Key = Encoding.UTF8.GetBytes(iconfiguration[SecretConfigKey]);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }


            return principal;
        }

      
        public bool ValidateToken(string token)
        {
            if (token == null)
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(iconfiguration["Jwt:Key"]);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "Id").Value);

                // return user id from JWT token if validation successful
                return true;
            }
            catch
            {
                // return null if validation fails
                return false;
            }
        }


        private SigningCredentials GetSigningCredentials()
        {
            if (_authSigningKey == null)
            {
                _authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(iconfiguration[SecretConfigKey]));
            }

            if (_signingCredentials == null)
            {
                _signingCredentials = new SigningCredentials(_authSigningKey, SecurityAlgorithms.HmacSha256);
            }

            return _signingCredentials;
        }


        public string DecodeAccessToken(string Token)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = handler.ReadJwtToken(Token);
            string AccessToken = jwtToken.Claims.First(claim => claim.Type == "AccessToken").Value;
            return AccessToken;

        }
        

        public string DecodeRefreshToken(string Token)
        {

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(iconfiguration[SecretConfigKey])),
                ValidIssuer = iconfiguration[ValidIssuerConfigKey],
                ValidateIssuer = false,
                ValidateAudience = false,
                TokenDecryptionKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(iconfiguration[SecretConfigKey])),
                ValidateLifetime = true
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(Token);
            string userid = jwtToken.Claims.First(claim => claim.Type == "Userid").Value;
            var principal = new JwtSecurityTokenHandler().ValidateToken(Token, tokenValidationParameters, out SecurityToken securityToken);
            return userid;
        }

       
    }

}
