using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using webApi2.Models;

namespace webApi2.JWT
{
    public class JwtUtils
    {

        private  const string secretKey="ihspidhasdASD2123123ASJDA123123SDKOQW@$%#ksdoh$%=)88619821$$%%132";
        private  const long timeMs=1800000;


        public JwtUtils()
        {
            
        }


        public string createToke(User user)
        {
            var key = Encoding.ASCII.GetBytes(secretKey);

            //crear los claims

            var claims = new ClaimsIdentity(new Claim[]
            {
                new Claim("Id",JsonConvert.SerializeObject(user.id.ToString())),
                new Claim("name",JsonConvert.SerializeObject(user.name)),
                new Claim("Role",JsonConvert.SerializeObject(user.isSuperUser))
            });


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                //Nuestro token expira en 30 minutos
                Expires = System.DateTime.UtcNow.AddMilliseconds(timeMs),

                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            

            var jwt = tokenHandler.CreateToken(tokenDescriptor);


            return tokenHandler.WriteToken(jwt);
        }
    }
}