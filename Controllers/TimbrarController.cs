using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webApi2.Service;
using webApi2.Service.InterfaceService;

namespace webApi2.Controllers
{
    [Route("api/v1/timbrar")]
    [ApiController]
    [Authorize]
    public class TimbrarController : ControllerBase
    {

        Itimbrar _itimbrar;
        public TimbrarController(Itimbrar _itimbrar)
        {
            this._itimbrar=_itimbrar;
        }


        [HttpPost]
        [Authorize(Policy="SuperUser")]
        public async Task<string> Post([FromForm] List<IFormFile> file,[FromForm] string password,[FromForm] string tipo,[FromForm] string certificado)
        {

            // if(complemento!=null)
            //     timbrar.variables.complemento=complemento;
            // else
            //     timbrar.variables.complemento="";
                     
            
            

            List<string> paths = new List<string>();
            //h:\\root\\home\\codestudio1-002\\www\\facturasjsonapi\\archivosyerrores
            //C:\\Users\\adrian\\Desktop\\prueba\\
            string filePath = "h:\\root\\home\\codestudio1-002\\www\\facturasjsonapi\\archivosyerrores";
            string pathKeys = "h:\\root\\home\\codestudio1-002\\www\\facturasjsonapi\\archivosyerrores";
            string pathCers = "h:\\root\\home\\codestudio1-002\\www\\facturasjsonapi\\archivosyerrores";
        
            
            //Save files
            var contextTxt = "";

            if (file.Count>0)
            {

                foreach(var fs in file)
                {
                        

                    if (fs.FileName.EndsWith(".json") || fs.FileName.EndsWith(".JSON"))
                    {
                        contextTxt += "\n" + fs.FileName;
                        filePath +=fs.FileName;
                        paths.Add(filePath);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await fs.CopyToAsync(stream);
                        }
                    }
                    else if (fs.FileName.EndsWith(".cer") || fs.FileName.EndsWith(".CER"))
                    {
                        contextTxt += "\n" + fs.FileName;
                        pathCers +=fs.FileName;
                        paths.Add(pathCers);
                        
                        using (var stream = System.IO.File.Create(pathCers))
                        {
                            await fs.CopyToAsync(stream);
                        }
                            
                    }
                    else if (fs.FileName.EndsWith(".key") || fs.FileName.EndsWith(".KEY")) 
                    {
                        contextTxt += "\n" + fs.FileName;
                        pathKeys +=fs.FileName;
                        paths.Add(pathKeys);
                        
                        using (var stream = System.IO.File.Create(pathKeys))
                        {
                            await fs.CopyToAsync(stream);
                        }
                            
                    }




                }

                

                //asignar las rutas donde estan los archivos y la contraseña
                Timbrar.variablesTimbrar.claveLLP = password;
                Timbrar.variablesTimbrar.rutaCer = pathCers;
                Timbrar.variablesTimbrar.rutaKey = pathKeys;

                var  xml = _itimbrar.readTXT(filePath, "", "contacto@codestudio.com.mx",tipo,password, "pruebas",certificado);
                


            
                

            

                //Eliminar los archivos almacenados
                if(certificado.Equals("si") && file.Count>1)
                {
                    System.IO.File.Delete(filePath);
                    System.IO.File.Delete(pathCers);
                    System.IO.File.Delete(pathKeys);
                }else{
                    System.IO.File.Delete(filePath);
               
                }
                


        

                return xml;
            }else{
                return "No enviaste archivos";
            }
        }
    }
}