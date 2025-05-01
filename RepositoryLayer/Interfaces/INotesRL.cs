using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using ModelLayer.Entity;
using RepositoryLayer.DTO;

namespace RepositoryLayer.Interfaces
{
    public interface INotesRL
    {
        Task<ResponseDTO<NotesEntity>> CreateNotes(NotesDTO notesModel, int userId);
        Task<ResponseDTO<IEnumerable<NotesEntity>>> RetriveNotes(int noteId, int userId);
        Task<ResponseDTO<IEnumerable<NotesEntity>>> RetriveAllNotes(int userId);
        Task<ResponseDTO<NotesEntity>> UpdateNote(int userId, int noteId, NotesDTO notes);
        Task<ResponseDTO<bool>> DeleteNote(int userId, int noteId);
        Task<ResponseDTO<bool>> TrashNote(int noteId, int userId);
        Task<ResponseDTO<bool>> PinNote(int noteId, int userId);
        Task<ResponseDTO<bool>> ArchiveNote(int userId, int noteId);
        Task<ResponseDTO<string>> BackgroundColorNote(int noteId, string backgroundColor);
        Task<ResponseDTO<string>> ImageNotes(IFormFile image, int noteId, int userId);
        Task<ResponseDTO<bool>> UnarchiveNote(int userId, int noteId);
        Task<ResponseDTO<bool>> RestoreNote(int noteId, int userId);
    }
}
