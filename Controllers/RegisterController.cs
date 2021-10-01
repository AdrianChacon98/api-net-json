using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using webApi2.Models.UserModel;
using webApi2.Service.InterfaceService;

namespace webApi2.Controllers
{
    [Route("api/v1/register")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        
        private IRegisterService _iRegisterService;
        

        public RegisterController(IRegisterService _iRegisterService)
        {
            this._iRegisterService=_iRegisterService;
        }



        [HttpPost]
        [Route("create")]
        public ActionResult<string> register([FromBody] UserModel userModel)
        {
            
            string response=_iRegisterService.register(userModel);
            
            if(response.Equals("El usuario no existe por favor cree un usuario"))
               return Ok(response);
            
            return Ok(response);
            
            
        }
        
    }
}