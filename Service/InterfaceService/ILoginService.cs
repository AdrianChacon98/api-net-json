using webApi2.Models.UserModel;

namespace webApi2.Service.InterfaceService
{
    public interface ILoginService
    {
         string signIn(UserLogin userLogin);
    }
}