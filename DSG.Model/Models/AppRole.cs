using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DSG.Model.Models
{
    [Table("AppRoles")]
    public class AppRole : IdentityRole
    {
        public AppRole() : base()
        {

        }
        [MaxLength(128)]
        public virtual string Description { get; set; }
    }
}
