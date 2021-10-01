using System.ComponentModel.DataAnnotations;

namespace webApi2.Models.Model
{
    public class VariablesTimbrar
    {
        [Key]
        public string rutaCer {set;get;}

        public string rutaKey {set;get;}
        public string error {set;get;}

        public string claveLLP {set;get;}

    }
}