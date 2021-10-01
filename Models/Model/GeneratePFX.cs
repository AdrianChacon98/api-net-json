using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Chilkat;


namespace webApi2.Models.Model
{
    class GeneratePFX
    {
       
     

        

        public  void ProcessCertificate(byte[] bytesPfx, string password)
        {
            var certificate2 = new X509Certificate2(bytesPfx, password);
            //Console.WriteLine($"Serial Number: {certificate2.SerialNumber}");
           // Console.WriteLine($"SubjectName: {certificate2.SubjectName.Name}");
           // Console.WriteLine($"NotBefore: {certificate2.NotAfter.ToString(CultureInfo.InvariantCulture)}");
           // Console.WriteLine($"NotAfter: {certificate2.NotAfter.ToString(CultureInfo.InvariantCulture)}");

            Debug.Assert(certificate2.HasPrivateKey, "Invalid pfx file...");
        }

        public  PrivateKey ComposePrivateKeyComponent(byte[] bytesKey, string password)
        {
            var privateKey = new PrivateKey();
            privateKey.LoadPkcs8Encrypted(bytesKey, password);
            return privateKey;
        }

        public string GetNewTempPathPfx()
        {
            return $"{Path.GetTempPath()}/{Guid.NewGuid()}.tmp";
        }

        public  Cert ComposeCertificateComponent(byte[] bytesCert, PrivateKey privateKey)
        {
            var cert = new Cert();
            cert.LoadFromBinary(bytesCert);
            cert.SetPrivateKey(privateKey);
            return cert;
        }
    }
}
