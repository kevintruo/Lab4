using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Lab4.Models
{
    public class Community
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Registration Number")]
        [Required]
        public string Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Budget { get; set; }

        //A collection of  navigation propertys
        public ICollection<CommunityMembership> Membership { get; set; }
    }
}