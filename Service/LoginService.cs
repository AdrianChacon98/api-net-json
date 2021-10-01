using System;
using System.Linq;
using System.Text.RegularExpressions;
using webApi2.Context;
using webApi2.JWT;
using webApi2.Models;
using webApi2.Models.UserModel;
using webApi2.Security;
using webApi2.Service.InterfaceService;

namespace webApi2.Service
{
    public class LoginService : ILoginService
    {
        private AppDbContext _appDbContext;

        public LoginService(AppDbContext _appDbContext)
        {
            this._appDbContext=_appDbContext;
        }

        
        public string signIn(UserLogin userLogin)
        {

            User user = new User();
            Encrypt encrypt = new Encrypt();

            string passwordEncrypted=encrypt.getSHA256(userLogin.password);

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(userLogin.email);

            try
            {
                if(match.Success)
                    user=_appDbContext.Users.First(u=>u.email.Equals(userLogin.email) && u.password.Equals(passwordEncrypted));
                else
                    return "El correo es incorrecto";

                    
            }catch(Exception e)
            {
                
            }

            if(user.email==null)
            {
                return "Tu correo o tu contraseña son incorrectas";
            }


            //crear el token
            JwtUtils jwt = new JwtUtils();

            string response = jwt.createToke(user);


            return response;
            
        }
    }
}