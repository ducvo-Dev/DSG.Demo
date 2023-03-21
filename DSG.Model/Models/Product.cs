using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using DSG.Model.Abstract;

namespace DSG.Model.Models
{
    [Table("Products")]
    public class Product : Auditable
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { set; get; }

        [Required]
        [MaxLength(256)]
        public string Name { set; get; }

        [Required]
        [MaxLength(256)]
        public string Alias { set; get; }

        [Required]
        public int CategoryID { set; get; }

        [MaxLength(256)]
        public string ThumbnailImage { set; get; }

        public decimal Price { set; get; }

        public decimal OriginalPrice { set; get; }

        public decimal? PromotionPrice { set; get; }

        public bool IncludedVAT { get; set; }


        [MaxLength(500)]
        public string Description { set; get; }

        public string Content { set; get; }
        public bool? HotFlag { set; get; }

        public int? ViewCount { set; get; }
        public string Tags { set; get; }

        [ForeignKey("CategoryID")]
        public virtual ProductCategory ProductCategory { set; get; }
    }
}
