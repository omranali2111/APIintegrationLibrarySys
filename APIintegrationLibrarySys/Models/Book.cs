﻿using System.ComponentModel.DataAnnotations;

namespace APIintegrationLibrarySys.models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        [MaxLength(255)]
        public string Author { get; set; }

        [Required]

        public int PublicationYear { get; set; }
        public bool IsAvailable { get; set; }

    }
}
