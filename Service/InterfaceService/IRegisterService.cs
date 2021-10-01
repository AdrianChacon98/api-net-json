using webApi2.Models.UserModel;

namespace webApi2.Service.InterfaceService
{
    public interface IRegisterService
    {
        string register(UserModel userModel);
    }
}