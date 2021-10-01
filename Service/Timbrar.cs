using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using SW.Helpers;
using SW.Services.Stamp;
using System.Xml.Xsl;
using System.Xml.Linq;
using SW.Services.Cancelation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using SW.Services.Status;

using System.Diagnostics;
using webApi2.Service.InterfaceService;
using webApi2.Models.Model;
using Newtonsoft.Json;
using System.Text.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using System.Xml.Serialization;

namespace webApi2.Service
{
    public class Timbrar: Itimbrar
    {

        
       

        //variable tipo factura
         public static VariablesTimbrar variablesTimbrar = new VariablesTimbrar();
        public const string PAGO="Pago";
        public const string SERVICIO_PARCIAL_DE_CONSTRUCCION="Servicios parciales de construcción";
        public const string COMERCIO_EXTERIOR="Comercio exterior";
        public const string LEYENDAS_FISCALES="Leyendas fiscales";
        public const string NOMINA="Nomina";
        public const string VALES_DE_DESPENSA="Vales de despensa";
        public const string NOTARIOS_PUBLICOS="Notarios publicos";
        public const string CARTA_PORTE="Carta porte";


        public string readTXT(string file, string cliente, string usuario,string tipo,string password, string servicio,string certificado)
        {

          //leer json
            try{
                
        
                //var comprobante= new Comprobante();
                string json = "";
                using (StreamReader jsonStream = new StreamReader(file,Encoding.Default))
                {
                    
                    json = jsonStream.ReadToEnd();
                    
                    //comprobante = JsonConvert.DeserializeObject<Comprobante>(json);
                }


                // string json2 = JsonConvert.SerializeObject(json);

                //para enviarlo con : revisar todos los formatos
                json=json.Replace("cfdi:","cfdi&");

                switch(tipo)
                {
                    case PAGO:
                        json=json.Replace("pago10:","pago10&");
                        break;
                    case SERVICIO_PARCIAL_DE_CONSTRUCCION:
                        json=json.Replace("servicioparcial:","servicioparcial&");
                        break;
                    case COMERCIO_EXTERIOR:
                        json=json.Replace("cce11:","cce11&");
                        break;
                    case LEYENDAS_FISCALES:
                        json=json.Replace("leyendasFisc:","leyendasFisc&");
                        break;
                    case NOMINA:
                        json=json.Replace("nomina12:","nomina12&");
                        break;
                    case VALES_DE_DESPENSA:
                        json=json.Replace("valesdedespensa:","valesdedespensa&");
                        break;
                    case NOTARIOS_PUBLICOS:
                        json=json.Replace("notariospublicos:","notariospublicos&");
                        break;
                    case CARTA_PORTE:
                        json=json.Replace("cartaporte:","cartaporte&");
                        break;
                }
                


                XmlDocument doc = new XmlDocument();
                doc =(XmlDocument)JsonConvert.DeserializeXmlNode(json,"cfdi&Comprobante",false,false);
                

                string xml="";
                try{
                    if(certificado.Equals("si"))
                        xml = timbrarConCertificados(doc,tipo, usuario, password, servicio);
                    else
                        xml = timbrarSinCertificados(doc,tipo, usuario, password, servicio);
                }catch(Exception e)
                {
                    xml="Error al timbrar el xml";
                }
                
                
            
                

                return xml;

            }catch(Exception e){
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
           
            
            return "hubo un error";

        }

        public string timbrarConCertificados(XmlDocument doc,string complemento,string usuario, string password,string servicio)
        {   
            //agregamos la declaracion xml
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                
            //seleccionamos nodo cfdi_x0026_Comprobante = cfdi:Comprobante
            XmlNode comprobanteXml = doc.SelectSingleNode("cfdi_x0026_Comprobante");


            //insertamos la declaracion xml antes que el nodo cfdi_x0026_Comprobante
            doc.InsertBefore(docNode,comprobanteXml);


            //crear atributos e insertarlos en el nodo comprobante
            XmlAttribute xmlns = doc.CreateAttribute("xmlns:cfdi");
            xmlns.Value="http://www.sat.gob.mx/cfd/3";
            comprobanteXml.Attributes.Append(xmlns);

             string schemaComplementos = null;
             
            switch (complemento)
            {
                case PAGO:
                    XmlAttribute pago = doc.CreateAttribute("xmlns:pago10");
                    pago.Value="http://www.sat.gob.mx/Pagos";
                    comprobanteXml.Attributes.Append(pago);
                    schemaComplementos = " http://www.sat.gob.mx/Pagos http://www.sat.gob.mx/sitio_internet/cfd/Pagos/Pagos10.xsd";
                    break;
                case SERVICIO_PARCIAL_DE_CONSTRUCCION:
                    XmlAttribute serviciosPC = doc.CreateAttribute("xmlns:servicioparcial");
                    serviciosPC.Value="http://www.sat.gob.mx/servicioparcialconstruccion";
                    comprobanteXml.Attributes.Append(serviciosPC);
                    schemaComplementos = " http://www.sat.gob.mx/servicioparcialconstruccion http://www.sat.gob.mx/sitio_internet/cfd/servicioparcialconstruccion/servicioparcialconstruccion.xsd";
                    break;
                case COMERCIO_EXTERIOR:
                    XmlAttribute cce = doc.CreateAttribute("xmlns:cce11");
                    cce.Value="http://www.sat.gob.mx/ComercioExterior11";
                    comprobanteXml.Attributes.Append(cce);
                    schemaComplementos = " http://www.sat.gob.mx/ComercioExterior11 http://www.sat.gob.mx/sitio_internet/cfd/ComercioExterior11/ComercioExterior11.xsd";
                    break;
                case LEYENDAS_FISCALES:
                    XmlAttribute leyendasFiscales = doc.CreateAttribute("xmlns:leyendasFisc");
                    leyendasFiscales.Value="http://www.sat.gob.mx/leyendasFiscales";
                    comprobanteXml.Attributes.Append(leyendasFiscales);
                    schemaComplementos = " http://www.sat.gob.mx/leyendasFiscales http://www.sat.gob.mx/sitio_internet/cfd/leyendasFiscales/leyendasFisc.xsd";
                    break;
                case NOMINA:
                    XmlAttribute nomina = doc.CreateAttribute("xmlns:nomina12");
                    nomina.Value="http://www.sat.gob.mx/nomina12";
                    comprobanteXml.Attributes.Append(nomina);
                    schemaComplementos = " http://www.sat.gob.mx/nomina12 http://www.sat.gob.mx/sitio_internet/cfd/nomina/nomina12.xsd";
                    break;
                case VALES_DE_DESPENSA:
                    schemaComplementos = " http://www.sat.gob.mx/valesdedespensa http://www.sat.gob.mx/sitio_internet/cfd/valesdedespensa/valesdedespensa.xsd";
                    break;
                case NOTARIOS_PUBLICOS:
                    schemaComplementos = " http://www.sat.gob.mx/sitio_internet/cfd/notariospublicos/notariospublicos.xsd";
                    break;
                case CARTA_PORTE:
                    XmlAttribute cartaPorte = doc.CreateAttribute("xmlns:cartaporte");
                    cartaPorte.Value="http://www.sat.gob.mx/CartaPorte";
                    comprobanteXml.Attributes.Append(cartaPorte);
                    schemaComplementos="http://www.sat.gob.mx/cfd/3 http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv33.xsd http://www.sat.gob.mx/CartaPorte http://www.sat.gob.mx/sitio_internet/cfd/CartaPorte/CartaPorte.xsd";
                    break;                            
            }

            XmlAttribute atrib_schemaLocation = doc.CreateAttribute("schemaLocation");
            atrib_schemaLocation.Value = "http://www.sat.gob.mx/cfd/3 http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv33.xsd" + schemaComplementos;
            comprobanteXml.Attributes.Append(atrib_schemaLocation);
                    
            XmlAttribute atrib_atrib_xmlns = doc.CreateAttribute("xsi");
            atrib_atrib_xmlns.Value = "http://www.w3.org/2001/XMLSchema-instance";
            comprobanteXml.Attributes.Append(atrib_atrib_xmlns);


            //generar certificado y numero del certificado
            byte[] bytesCer;
            byte[] bytesKey;
            string certTxt = null;


            bytesCer = System.IO.File.ReadAllBytes(variablesTimbrar.rutaCer);
            bytesKey = System.IO.File.ReadAllBytes(variablesTimbrar.rutaKey);

            try
            {
                //Obtenemos el certificado en Texto
                X509Certificate2 x509 = new X509Certificate2(File.ReadAllBytes(variablesTimbrar.rutaCer));
                //Create X509Certificate2 object from .cer file.
                //byte[] rawData = ReadFile(rutaCer);
                //x509.Import(bytesCer);

                certTxt = ExportToPEM(bytesCer);
                XmlAttribute atribute_Cer = doc.CreateAttribute("Certificado");
                atribute_Cer.Value = certTxt;
                comprobanteXml.Attributes.Append(atribute_Cer);
                //Sacamos el Numero del Certificado
                string NumCert = ObtenerNumeroCertificado(x509);
                XmlAttribute atribute_numCer = doc.CreateAttribute("NoCertificado");
                atribute_numCer.Value = NumCert;
                comprobanteXml.Attributes.Append(atribute_numCer);
            }
            catch (Exception e)
            {
                variablesTimbrar.error += "Verifique el Certificado no " + variablesTimbrar.rutaCer + " cert " + Environment.NewLine +e.Message;
            }


            string Xmlstring = doc.OuterXml;
            Xmlstring = Xmlstring.Replace("xsi=", "xmlns:xsi=");
            Xmlstring = Xmlstring.Replace("schemaLocation=", "xsi:schemaLocation=");
            Xmlstring = Xmlstring.Replace("_x0026_",":");


            //generar pfx
            GeneratePFX generatePFX = new GeneratePFX();
                   

            var pfxPath = generatePFX.GetNewTempPathPfx();

            var privateKey = generatePFX.ComposePrivateKeyComponent(bytesKey, password);

            var cert = generatePFX.ComposeCertificateComponent(bytesCer, privateKey);

            //Exportar en archivo certificado Pfx.
            var success = cert.ExportToPfxFile(pfxPath, password, false);

                    


            Debug.Assert(success, "Error processing certificate..");

            byte[] pfx = File.ReadAllBytes(pfxPath);

                    

            File.Delete(pfxPath);

            //Procesar archivo PFX
            generatePFX.ProcessCertificate(pfx, password);

            //verifica Complemento

            //Pendiente el tema de los complementos
            string genxml = null;
            try
            {
                if (!string.IsNullOrEmpty(complemento))
                {
                    
                    genxml = Xmlstring;//validaComplemento(Xmlstring, complemento);
                }
                else
                {
                    genxml = Xmlstring;
                }
            }
            catch
            {
                genxml = Xmlstring;
            }
            


            //timbrar xml
            string server = null;
            string resXML = null;

            try
            {
                //Ejemplo Timbrado utilizando la librería sw-sdk ( https://www.nuget.org/packages/SW-sdk/ )
                //Para mayor referencia: https://github.com/lunasoft/sw-sdk-dotnet
                //Creamos una instancia de tipo Stamp 
                //A esta le pasamos la Url, Usuario y Contraseña para obtener el token
                //Automaticamente despues de obtenerlo se procedera a timbrar el xml
                       
                if (servicio == "produccion")
                {
                    server = "http://services.sw.com.mx";
                }
                else
                {
                    server = "http://services.test.sw.com.mx";
                    usuario = "contacto@codestudio.com.mx";
                    password = "QWEqwe123##";
                }                        
                if (string.IsNullOrEmpty(variablesTimbrar.error))
                {

                            
                    Stamp stamp = new Stamp(server, usuario, password);
                            

                    string CO = null;
                    try
                    {                                
                        CO = "123";

                        if (CO == "")
                        {
                            variablesTimbrar.error += "Error al generar la Cadena Original Verifique la ruta del XSLT" + Environment.NewLine;
                        }
                    }
                    catch (Exception R)
                    {
                        variablesTimbrar.error += "Error al generar la Cadena Original" + Environment.NewLine;
                    }
                    
                    
                    string lmx="";
                    try{

                        /*Console.WriteLine(genxml);
                        Console.WriteLine(Encoding.UTF8.GetString(pfx));
                        Console.WriteLine(CO);*/

                        lmx = createSello(genxml, pfx, CO);
                        
                        
                    }catch(Exception e)
                    {
                            Console.WriteLine(e.StackTrace);
                    }


                    StampResponseV4 responseCO = stamp.TimbrarV4(lmx);


                    try
                    {                                
                        try
                        {            
                            
                            XmlDocument d = new XmlDocument();
                            CO = responseCO.messageDetail;
                            d.LoadXml(responseCO.messageDetail);
                            resXML = responseCO.messageDetail;  
                                                                          
                        }
                        catch
                        {
                            if(!string.IsNullOrEmpty(responseCO.messageDetail))
                            { 
                                CO = responseCO.messageDetail;
                            }

                            string xlm = createSello(genxml, pfx, CO.Replace("CadenaOriginal: ", ""));
                            StampResponseV4 response = stamp.TimbrarV4(xlm);
                            XmlDocument d = new XmlDocument();

                            try
                            {                                        
                                        
                                d.LoadXml(response.data.cfdi);
                                resXML = response.data.cfdi;                                        
                            }
                            catch
                            {
                                try
                                { 
                                    d.LoadXml(response.messageDetail);
                                    resXML = response.messageDetail;
                                }
                                catch
                                {
                                    resXML = response.messageDetail + " " + response.message + Environment.NewLine + genxml;// + genxml
                                }
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        resXML = responseCO.message + " " + responseCO.messageDetail + Environment.NewLine + genxml;// + genxml

        
                    }
                }
                else { resXML = " verifique los siguientes campos:" + variablesTimbrar.error + Environment.NewLine + genxml; }//+ genxml
                        //lmx
            }
            catch (Exception e)
            {
                
            }

            variablesTimbrar=new VariablesTimbrar();


            return resXML;
        }

        public string timbrarSinCertificados(XmlDocument doc,string complemento,string usuario, string password,string servicio)
        {   
            //agregamos la declaracion xml
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                
            //seleccionamos nodo cfdi_x0026_Comprobante = cfdi:Comprobante
            XmlNode comprobanteXml = doc.SelectSingleNode("cfdi_x0026_Comprobante");


            //insertamos la declaracion xml antes que el nodo cfdi_x0026_Comprobante
            doc.InsertBefore(docNode,comprobanteXml);


            //crear atributos e insertarlos en el nodo comprobante
            XmlAttribute xmlns = doc.CreateAttribute("xmlns:cfdi");
            xmlns.Value="http://www.sat.gob.mx/cfd/3";
            comprobanteXml.Attributes.Append(xmlns);

             string schemaComplementos = null;
             
            switch (complemento)
            {
                case PAGO:
                    XmlAttribute pago = doc.CreateAttribute("xmlns:pago10");
                    pago.Value="http://www.sat.gob.mx/Pagos";
                    comprobanteXml.Attributes.Append(pago);
                    schemaComplementos = " http://www.sat.gob.mx/Pagos http://www.sat.gob.mx/sitio_internet/cfd/Pagos/Pagos10.xsd";
                    break;
                case SERVICIO_PARCIAL_DE_CONSTRUCCION:
                    XmlAttribute serviciosPC = doc.CreateAttribute("xmlns:servicioparcial");
                    serviciosPC.Value="http://www.sat.gob.mx/servicioparcialconstruccion";
                    comprobanteXml.Attributes.Append(serviciosPC);
                    schemaComplementos = " http://www.sat.gob.mx/servicioparcialconstruccion http://www.sat.gob.mx/sitio_internet/cfd/servicioparcialconstruccion/servicioparcialconstruccion.xsd";
                    break;
                case COMERCIO_EXTERIOR:
                    XmlAttribute cce = doc.CreateAttribute("xmlns:cce11");
                    cce.Value="http://www.sat.gob.mx/ComercioExterior11";
                    comprobanteXml.Attributes.Append(cce);
                    schemaComplementos = " http://www.sat.gob.mx/ComercioExterior11 http://www.sat.gob.mx/sitio_internet/cfd/ComercioExterior11/ComercioExterior11.xsd";
                    break;
                case LEYENDAS_FISCALES:
                    XmlAttribute leyendasFiscales = doc.CreateAttribute("xmlns:leyendasFisc");
                    leyendasFiscales.Value="http://www.sat.gob.mx/leyendasFiscales";
                    comprobanteXml.Attributes.Append(leyendasFiscales);
                    schemaComplementos = " http://www.sat.gob.mx/leyendasFiscales http://www.sat.gob.mx/sitio_internet/cfd/leyendasFiscales/leyendasFisc.xsd";
                    break;
                case NOMINA:
                    XmlAttribute nomina = doc.CreateAttribute("xmlns:nomina12");
                    nomina.Value="http://www.sat.gob.mx/nomina12";
                    comprobanteXml.Attributes.Append(nomina);
                    schemaComplementos = " http://www.sat.gob.mx/nomina12 http://www.sat.gob.mx/sitio_internet/cfd/nomina/nomina12.xsd";
                    break;
                case VALES_DE_DESPENSA:
                    schemaComplementos = " http://www.sat.gob.mx/valesdedespensa http://www.sat.gob.mx/sitio_internet/cfd/valesdedespensa/valesdedespensa.xsd";
                    break;
                case NOTARIOS_PUBLICOS:
                    schemaComplementos = " http://www.sat.gob.mx/sitio_internet/cfd/notariospublicos/notariospublicos.xsd";
                    break;
                case CARTA_PORTE:
                    XmlAttribute carta = doc.CreateAttribute("xmlns:cartaporte");
                    carta.Value="http://www.sat.gob.mx/CartaPorte";
                    comprobanteXml.Attributes.Append(carta);
                    schemaComplementos = "http://www.sat.gob.mx/cfd/3 http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv33.xsd http://www.sat.gob.mx/CartaPorte http://www.sat.gob.mx/sitio_internet/cfd/CartaPorte/CartaPorte.xsd";
                    break;                           
            }

            XmlAttribute atrib_schemaLocation = doc.CreateAttribute("schemaLocation");
            atrib_schemaLocation.Value = "http://www.sat.gob.mx/cfd/3 http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv33.xsd" + schemaComplementos;
            comprobanteXml.Attributes.Append(atrib_schemaLocation);
                    
            XmlAttribute atrib_atrib_xmlns = doc.CreateAttribute("xsi");
            atrib_atrib_xmlns.Value = "http://www.w3.org/2001/XMLSchema-instance";
            comprobanteXml.Attributes.Append(atrib_atrib_xmlns);

            /*
            //generar certificado y numero del certificado
            byte[] bytesCer;
            byte[] bytesKey;
            string certTxt = null;


            bytesCer = System.IO.File.ReadAllBytes(variablesTimbrar.rutaCer);
            bytesKey = System.IO.File.ReadAllBytes(variablesTimbrar.rutaKey);

            try
            {
                //Obtenemos el certificado en Texto
                X509Certificate2 x509 = new X509Certificate2(File.ReadAllBytes(variablesTimbrar.rutaCer));
                //Create X509Certificate2 object from .cer file.
                //byte[] rawData = ReadFile(rutaCer);
                //x509.Import(bytesCer);

                certTxt = ExportToPEM(bytesCer);
                XmlAttribute atribute_Cer = doc.CreateAttribute("Certificado");
                atribute_Cer.Value = certTxt;
                comprobanteXml.Attributes.Append(atribute_Cer);
                //Sacamos el Numero del Certificado
                string NumCert = ObtenerNumeroCertificado(x509);
                XmlAttribute atribute_numCer = doc.CreateAttribute("NoCertificado");
                atribute_numCer.Value = NumCert;
                comprobanteXml.Attributes.Append(atribute_numCer);
            }
            catch (Exception e)
            {
                variablesTimbrar.error += "Verifique el Certificado no " + variablesTimbrar.rutaCer + " cert " + Environment.NewLine +e.Message;
            }
            */

            string Xmlstring = doc.OuterXml;
            Xmlstring = Xmlstring.Replace("xsi=", "xmlns:xsi=");
            Xmlstring = Xmlstring.Replace("schemaLocation=", "xsi:schemaLocation=");
            Xmlstring = Xmlstring.Replace("_x0026_",":");

            /*
            //generar pfx
            GeneratePFX generatePFX = new GeneratePFX();
                   

            var pfxPath = generatePFX.GetNewTempPathPfx();

            var privateKey = generatePFX.ComposePrivateKeyComponent(bytesKey, password);

            var cert = generatePFX.ComposeCertificateComponent(bytesCer, privateKey);

            //Exportar en archivo certificado Pfx.
            var success = cert.ExportToPfxFile(pfxPath, password, false);

                    


            Debug.Assert(success, "Error processing certificate..");

            byte[] pfx = File.ReadAllBytes(pfxPath);

                    

            File.Delete(pfxPath);

            //Procesar archivo PFX
            generatePFX.ProcessCertificate(pfx, password);
            */
            //verifica Complemento

            //Pendiente el tema de los complementos
            string genxml = null;
            try
            {
                if (!string.IsNullOrEmpty(complemento))
                {
                    
                    genxml = Xmlstring;//validaComplemento(Xmlstring, complemento);
                }
                else
                {
                    genxml = Xmlstring;
                }
            }
            catch
            {
                genxml = Xmlstring;
            }
            


            //timbrar xml
            string server = null;
            string resXML = null;

            try
            {
                //Ejemplo Timbrado utilizando la librería sw-sdk ( https://www.nuget.org/packages/SW-sdk/ )
                //Para mayor referencia: https://github.com/lunasoft/sw-sdk-dotnet
                //Creamos una instancia de tipo Stamp 
                //A esta le pasamos la Url, Usuario y Contraseña para obtener el token
                //Automaticamente despues de obtenerlo se procedera a timbrar el xml
                       
                if (servicio == "produccion")
                {
                    server = "http://services.sw.com.mx";
                }
                else
                {
                    server = "http://services.test.sw.com.mx";
                    usuario = "contacto@codestudio.com.mx";
                    password = "QWEqwe123##";
                }                        
                if (string.IsNullOrEmpty(variablesTimbrar.error))
                {

                            
                    Stamp stamp = new Stamp(server, usuario, password);
                            

                    string CO = null;
                    try
                    {                                
                        CO = "123";

                        if (CO == "")
                        {
                            variablesTimbrar.error += "Error al generar la Cadena Original Verifique la ruta del XSLT" + Environment.NewLine;
                        }
                    }
                    catch (Exception R)
                    {
                        variablesTimbrar.error += "Error al generar la Cadena Original" + Environment.NewLine;
                    }
                    
                    
                    string lmx="";
                    try{

                        /*Console.WriteLine(genxml);
                        Console.WriteLine(Encoding.UTF8.GetString(pfx));
                        Console.WriteLine(CO);*/

                        //lmx = createSello(genxml, pfx, CO);
                        lmx=genxml;
                        
                    }catch(Exception e)
                    {
                            Console.WriteLine(e.StackTrace);
                    }


                    StampResponseV4 responseCO = stamp.TimbrarV4(lmx);


                    try
                    {                                
                        try
                        {            
                            
                            XmlDocument d = new XmlDocument();
                            CO = responseCO.messageDetail;
                            d.LoadXml(responseCO.messageDetail);
                            resXML = responseCO.messageDetail;  
                                                                          
                        }
                        catch
                        {
                            if(!string.IsNullOrEmpty(responseCO.messageDetail))
                            { 
                                CO = responseCO.messageDetail;
                            }

                            // string xlm = createSello(genxml, pfx, CO.Replace("CadenaOriginal: ", ""));
                            StampResponseV4 response = stamp.TimbrarV4(genxml);
                            XmlDocument d = new XmlDocument();

                            try
                            {                                        
                                        
                                d.LoadXml(response.data.cfdi);
                                resXML = response.data.cfdi;                                        
                            }
                            catch
                            {
                                try
                                { 
                                    d.LoadXml(response.messageDetail);
                                    resXML = response.messageDetail;
                                }
                                catch
                                {
                                    resXML = response.messageDetail + " " + response.message + Environment.NewLine + genxml;// + genxml
                                }
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        resXML = responseCO.message + " " + responseCO.messageDetail + Environment.NewLine + genxml;// + genxml

                        
                    }
                }
                else { resXML = " verifique los siguientes campos:" + variablesTimbrar.error + Environment.NewLine + genxml; }//+ genxml
                        //lmx
            }
            catch (Exception e)
            {
                
            }

            variablesTimbrar=new VariablesTimbrar();


            return resXML;
        }

        private string createSello(string xml, byte[] pfx, string CO)
        {

            
            
            string resp = null;
            try
            {
                var xmlResult = ObtenerSello(pfx, variablesTimbrar.claveLLP, CO);
                
                //sello = ObtenerSello(CO.Trim());
                XmlDocument doc2 = new XmlDocument();
                doc2.LoadXml(xml.ToString());
                //Agregamos el NameSpace para volver a leer el nodo principal, comprobante
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc2.NameTable);
                //Agregamos un namespace,
                nsmgr.AddNamespace("cfdi", "http://www.sat.gob.mx/cfd/3");
                //Seleccionamos el nodo comprobante
                XmlNode book = doc2.SelectSingleNode("//cfdi:Comprobante", nsmgr);
                //Creamos el atributo correspondiente.
                XmlAttribute atribute_sello = doc2.CreateAttribute("Sello");
                //Aplicamos el valor de sello al atributo
                atribute_sello.Value = xmlResult;
                book.Attributes.Append(atribute_sello);
                //genxml = genxml.Replace("xsi=", "xmlns:xsi=");
                //genxml = genxml.Replace("schemaLocation=", "xsi:schemaLocation="); 
                resp = doc2.OuterXml;
            }
            catch (Exception e)
            {
                    //C:\Users\adrian\Desktop\prueba\exception.log
                    //h:\root\home\codestudio1-002\www\facturasjsonapi\archivosyerrores\exceptionSello.log
                    string pathlog = @"h:\root\home\codestudio1-002\www\facturasjsonapi\archivosyerrores\exceptionSello.log";
                    StreamWriter sw = File.CreateText(pathlog);
                    sw.Close();

                    

                    File.AppendAllText(pathlog, e.Message.ToString());
                    File.AppendAllText(pathlog, e.StackTrace.ToString());
                    variablesTimbrar.error += "Error al generar el Sello" + Environment.NewLine;
                
                return "Error en el tipo de factura";

            }

            return resp;
        }

        public static string ObtenerSello(byte[] certificatePfx, string password, string cadenaOriginal)
        {
            //Sellamos la factura con el CSD y la cadena original aplicando el algoritmo SHA256
            var signData = string.Empty;
            RSACryptoServiceProvider rsa = default(RSACryptoServiceProvider);
            byte[] signatureBytes = default(byte[]);
            X509Certificate2 certX509 = new X509Certificate2(certificatePfx, password
                 , X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable);

            rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(certX509.PrivateKey.ToXmlString(true));

            byte[] data = Encoding.UTF8.GetBytes(cadenaOriginal);

            signatureBytes = rsa.SignData(data, CryptoConfig.MapNameToOID("SHA256"));
            return Convert.ToBase64String(signatureBytes);

        }

        public string ExportToPEM(byte[] cert)
        {
            return Convert.ToBase64String(cert); ;
        }

        public static string ObtenerNumeroCertificado(X509Certificate2 cert)
        {
            string hexadecimalString = cert.SerialNumber;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i <= hexadecimalString.Length - 2; i += 2)
            {
                sb.Append(Convert.ToString(Convert.ToChar(Int32.Parse(hexadecimalString.Substring(i, 2), System.Globalization.NumberStyles.HexNumber))));
            }
            return sb.ToString();
        }
       
    }
}