using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModelLayer.Entity
{
    public class NoteCollaboratorEntity
    {
        [Key]
        public int Id { get; set; }

        public int NoteId { get; set; }

        [ForeignKey("NoteId")]
        public NotesEntity Note { get; set; }

        public int UserId { get; set; }  

        [ForeignKey("UserId")]
        public UserEntity User { get; set; }
    }
}
