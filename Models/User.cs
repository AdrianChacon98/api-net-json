using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApi2.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id {set;get;}
        [Required]
        [MaxLength(50)]        
        public string name {set; get;}
        [Required]
        [MaxLength(50)]
        public string lastName {set; get;}
        [Required]
        [MaxLength(100)]
        public string email {set; get;}
        [Required]
        [MaxLength(255)]
        public string password {set; get;}
        
        public bool isSuperUser {get;set;}

        [Required]
        [Column("dateCreate")]
        public DateTime dateCreated {set; get;} 

    }
}