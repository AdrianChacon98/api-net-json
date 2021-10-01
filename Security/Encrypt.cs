using System.Security.Cryptography;
using System.Text;

namespace webApi2.Security
{
    public class Encrypt
    {
        public string getSHA256(string password)
        {
            SHA256 sha256=SHA256Managed.Create();
 
            ASCIIEncoding encoding = new ASCIIEncoding();

            byte [] stream =null;

            StringBuilder sb = new StringBuilder();

            stream = sha256.ComputeHash(encoding.GetBytes(password));

            for(int i=0;i<stream.Length;i++)
            {
                sb.AppendFormat("{0:x2}",stream[i]);
            }


            return sb.ToString();
        }
    }
}