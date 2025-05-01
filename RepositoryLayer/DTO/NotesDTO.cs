using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Entity;

namespace RepositoryLayer.DTO
{
    public class NotesDTO
    {
        [Key]      

        public string Title { get; set; }
        public string Description { get; set; }
        public string Backgroundcolor { get; set; }
        public string? Image { get; set; }
        public bool Pin { get; set; }
        public bool Trash { get; set; }
        public bool Archieve { get; set; }
    }
}
