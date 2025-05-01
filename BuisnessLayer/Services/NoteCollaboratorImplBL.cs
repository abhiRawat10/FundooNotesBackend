using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuisnessLayer.Interfaces;
using BuisnessLayer.InterFaces;
using RepositoryLayer.DTO;
using RepositoryLayer.Interfaces;

namespace BuisnessLayer.Services
{
    public class NoteCollaboratorImplBL : INoteCollaboratorBL
    {
        private readonly INoteCollaboratorRL _noteCollaboratorRL;
        public NoteCollaboratorImplBL(INoteCollaboratorRL noteCollaboratorRL)
        {
            this._noteCollaboratorRL = noteCollaboratorRL;
        }
        public async Task<ResponseDTO<bool>> ShareNote(int noteId, string collaboratorEmail)
        {
            try
            {
                var result = await _noteCollaboratorRL.ShareNote(noteId, collaboratorEmail);
                if (result.success)
                {
                    return new ResponseDTO<bool>
                    {
                        success = true,
                        message = "Note Shared Successfully",
                        data = result.data
                    };
                }
                else
                {
                    return new ResponseDTO<bool>
                    {
                        success = false,
                        message = "Note Share Failed",
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
