using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lab4.Models
{
    public class Student
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Enrollment Date")]
        public DateTime EnrollmentDate { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get { return LastName + ", " + FirstName; } }

        //A collection of navigation property
        public ICollection<CommunityMembership> Membership { get; set; }
    }
}