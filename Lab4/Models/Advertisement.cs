﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lab4.Models
{
    public class Advertisement
    {
        public int AdvertisementId { get; set; }

        [Required]
        [DisplayName("File Name")]
        public string FileName { get; set; }

        [Required]
        [Url]
        public string Url{ get; set; }

        [Required]
        public Community community;
    }
}
