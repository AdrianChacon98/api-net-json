using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using webApi2.Models.UserModel;
using webApi2.Service.InterfaceService;

namespace webApi2.Controllers
{
    [Route("api/v1/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private ILoginService _iLoginService;
        
        public LoginController(ILoginService _iLoginService)
        {
            this._iLoginService=_iLoginService;
        }
        

        [HttpPost]
        [Route("signin")]
        public ActionResult<string> signIn(UserLogin userLogin)
        {
            string reponse = _iLoginService.signIn(userLogin);
            
            return reponse;
        }

    }
}