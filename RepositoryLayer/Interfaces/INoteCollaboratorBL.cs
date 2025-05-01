using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.DTO;

namespace RepositoryLayer.Interfaces
{
    public interface INoteCollaboratorRL
    {
        Task<ResponseDTO<bool>> ShareNote(int noteId, string collaboratorEmail);
    }
}
