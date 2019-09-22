
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Seagull.API.APIHelper.Authorization
{
    public class Token
    {
        public static string GenerateToken(string UserId, string UserRole)
        {
            APIJsonResult data = new APIJsonResult();
            //security key
            string securityKey = "this_is_our_supper_long_security_key_for_token_validation_project_2018_09_07$smesk.in";
            //symmetric security key
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));

            //signing credentials
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            //add claims
            var claims = new List<Claim>();

            claims.Add(new Claim("UserId", UserId.ToString()));
            claims.Add(new Claim("UserRole", UserRole));
            
            
            //create token
            var token = new JwtSecurityToken(
                    issuer: "smesk.in",
                    audience: "readers",
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: signingCredentials
                    , claims: claims
                );
            //return token
            //HttpContext.Session.SetString("token", JsonConvert.SerializeObject(token));

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
