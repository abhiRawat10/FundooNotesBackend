using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuisnessLayer.InterFaces;
using ModelLayer.Entity;
using RepositoryLayer.DTO;
using RepositoryLayer.Interfaces;
using RepositoryLayer.Services;
using RepositoryLayer.Interfaces;
using Microsoft.AspNetCore.Http;

namespace BuisnessLayer.Services
{
    public class NotesImplBL : INotesBL
    {
        private readonly INotesRL _notesRL;
        public NotesImplBL(INotesRL notesRL)
        {
            _notesRL = notesRL;
        }

        public async Task<ResponseDTO<NotesEntity>> CreateNotes(NotesDTO notesModel, int userId)
        {
            try
            {
                var result = await _notesRL.CreateNotes(notesModel, userId);
                if (result.success)
                {
                    return new ResponseDTO<NotesEntity>
                    {
                        success = true,
                        message = "Note Created Successfully",
                        data = result.data
                    };
                }
                else
                {
                    return new ResponseDTO<NotesEntity>
                    {
                        success = false,
                        message = "Note Creation Failed",
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public async Task<ResponseDTO<IEnumerable<NotesEntity>>> RetriveNotes(int noteId, int userId)
        {
            try
            {
                var result = await _notesRL.RetriveNotes(noteId, userId);
                if (result.success)
                {
                    return new ResponseDTO<IEnumerable<NotesEntity>>
                    {
                        success = true,
                        message = "Note Retrieved Successfully",
                        data = result.data
                    };
                }
                else
                {
                    return new ResponseDTO<IEnumerable<NotesEntity>>
                    {
                        success = false,
                        message = "Note Retrieval Failed",
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseDTO<IEnumerable<NotesEntity>>> RetriveAllNotes(int userId)
        {
            try
            {
                var result = await _notesRL.RetriveAllNotes(userId);
                if (result.success)
                {
                    return new ResponseDTO<IEnumerable<NotesEntity>>
                    {
                        success = true,
                        message = "All Notes Retrieved Successfully",
                        data = result.data
                    };
                }
                else
                {
                    return new ResponseDTO<IEnumerable<NotesEntity>>
                    {
                        success = false,
                        message = "Note Retrieval Failed",
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public async Task<ResponseDTO<NotesEntity>> UpdateNote(int userId, int noteId, NotesDTO notes)
        {
            try
            {
                var result = await _notesRL.UpdateNote(userId, noteId, notes);
                if (result.success)
                {
                    return new ResponseDTO<NotesEntity>
                    {
                        success = true,
                        message = "Note Updated Successfully",
                        data = result.data
                    };
                }
                else
                {
                    return new ResponseDTO<NotesEntity>
                    {
                        success = false,
                        message = "Note Update Failed",
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseDTO<bool>> DeleteNote(int userId, int noteId)
        {
            try
            {
                var result = await _notesRL.DeleteNote(userId, noteId);
                if (result.success)
                {
                    return new ResponseDTO<bool>
                    {
                        success = true,
                        message = "Note Deleted Successfully",
                        data = result.data
                    };
                }
                else
                {
                    return new ResponseDTO<bool>
                    {
                        success = false,
                        message = "Note Deletion Failed",
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ResponseDTO<bool>> TrashNote(int noteId, int userId)
        {
            try
            {
                var result = await _notesRL.TrashNote(noteId, userId);
                if (result.success)
                {
                    return new ResponseDTO<bool>
                    {
                        success = true,
                        message = "Note Trashed Successfully",
                        data = result.data
                    };
                }
                else
                {
                    return new ResponseDTO<bool>
                    {
                        success = false,
                        message = "Note Trash Failed",
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseDTO<bool>> PinNote(int noteId, int userId)
        {
            try
            {
                var result = await _notesRL.PinNote(noteId, userId);
                if (result.success)
                {
                    return new ResponseDTO<bool>
                    {
                        success = true,
                        message = "Note Pinned Successfully",
                        data = result.data
                    };
                }
                else
                {
                    return new ResponseDTO<bool>
                    {
                        success = false,
                        message = "Note Pin Failed",
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseDTO<bool>> ArchiveNote(int userId, int noteId)
        {
            try
            {
                var result = await _notesRL.ArchiveNote(userId, noteId);
                if (result.success)
                {
                    return new ResponseDTO<bool>
                    {
                        success = true,
                        message = "Note Archived Successfully",
                        data = result.data
                    };
                }
                else
                {
                    return new ResponseDTO<bool>
                    {
                        success = false,
                        message = "Note Archive Failed",
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseDTO<string>> BackgroundColorNote(int noteId, string backgroundColor)
        {
            try
            {
                var result = await _notesRL.BackgroundColorNote(noteId, backgroundColor);
                if (result.success)
                {
                    return new ResponseDTO<string>
                    {
                        success = true,
                        message = "Background Color Changed Successfully",
                        data = result.data
                    };
                }
                else
                {
                    return new ResponseDTO<string>
                    {
                        success = false,
                        message = "Background Color Change Failed",
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseDTO<string>> ImageNotes(IFormFile image, int noteId, int userId)
        {
            try
            {
                var result = await _notesRL.ImageNotes(image, noteId, userId);
                if (result.success)
                {
                    return new ResponseDTO<string>
                    {
                        success = true,
                        message = "Image Added Successfully",
                        data = result.data
                    };
                }
                else
                {
                    return new ResponseDTO<string>
                    {
                        success = false,
                        message = "Image Addition Failed",
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseDTO<bool>> UnarchiveNote(int userId, int noteId)
        {
            try
            {
                var result = await _notesRL.UnarchiveNote(userId, noteId);
                if (result.success)
                {
                    return new ResponseDTO<bool>
                    {
                        success = true,
                        message = "Note Unarchived Successfully",
                        data = result.data
                    };
                }
                else
                {
                    return new ResponseDTO<bool>
                    {
                        success = false,
                        message = "Note Unarchive Failed",
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseDTO<bool>> RestoreNote(int noteId, int userId)
        {
            try
            {
                var result = await _notesRL.RestoreNote(noteId, userId);
                if (result.success)
                {
                    return new ResponseDTO<bool>
                    {
                        success = true,
                        message = "Note Restored Successfully",
                        data = result.data
                    };
                }
                else
                {
                    return new ResponseDTO<bool>
                    {
                        success = false,
                        message = "Note Restore Failed",
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
