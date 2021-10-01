using System;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using webApi2.Context;
using webApi2.Models;
using webApi2.Models.UserModel;
using webApi2.Security;
using webApi2.Service.InterfaceService;

namespace webApi2.Service
{
    public class RegisterService : IRegisterService
    {

        AppDbContext _appDbContext;

        
        public RegisterService(AppDbContext _appDbContext)
        {
            this._appDbContext=_appDbContext;
        }



        public string register(UserModel userModel)
        {
            
            User user = new User();
            Encrypt encrypt = new Encrypt();

            //validar email usar regex
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(userModel.email);

            try
            {
                if(match.Success)
                    user = _appDbContext.Users.First(u=>u.email.Equals(userModel.email));
                else
                    return "El email no es valido";    

            }catch(Exception e)
            {
                
            }

            if(user.email!=null)
            {
                return "El usuario ya existe";
            }
            
            try
            {
                user.name=userModel.name;
                user.lastName=userModel.lastName;
                user.email=userModel.email;
                user.password=encrypt.getSHA256(userModel.password);
                

                Role role = new Role();
                role.id=1;
                role.name="user";


                user.isSuperUser=false;
                user.dateCreated=DateTime.UtcNow;

                _appDbContext.Users.Add(user);
                _appDbContext.SaveChanges();


            }catch(Exception e)
            {
            
                return "No se pudo registrar el usuario";
                
            }
            
            return "Registro exitoso";
        }

        
    }
}