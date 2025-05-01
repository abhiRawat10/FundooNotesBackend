using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.DTO;

namespace BuisnessLayer.Interfaces
{
    public interface INoteCollaboratorBL
    {
        Task<ResponseDTO<bool>> ShareNote(int noteId, string collaboratorEmail);
    }
}
