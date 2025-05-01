using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Entity;
using NLog;
using RepositoryLayer.Context;
using RepositoryLayer.DTO;
using RepositoryLayer.Interfaces;
using StackExchange.Redis;

namespace RepositoryLayer.Services
{
    public class NotesImplRL : INotesRL
    {
        public UserContext _context;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IDatabase _redisDb;

        public NotesImplRL(UserContext context, IDatabase redisDb)
        {
            _context = context;
            _redisDb = redisDb;
        }

        public async Task<ResponseDTO<NotesEntity>> CreateNotes(NotesDTO notesModel, int userId)
        {
            try
            {
                NotesEntity note = new NotesEntity()
                {
                    UserId = userId,
                    Title = notesModel.Title,
                    Description = notesModel.Description,
                    Reminder = null,
                    Backgroundcolor = notesModel.Backgroundcolor,
                    Image = null,
                    Pin = notesModel.Pin,
                    Created = DateTime.Now,
                    Edited = DateTime.Now,
                    Trash = false,
                    Archieve = false
                };
                _context.Notes.Add(note);
                _context.SaveChanges();

                await _redisDb.KeyDeleteAsync($"notes:user:{userId}");
                logger.Info($"Note created for UserId: {userId}");

                return new ResponseDTO<NotesEntity>()
                {
                    success = true,
                    message = "Note Created Successfully",
                    data = note
                };
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error occurred while creating note.");
                throw;
            }
        }



        public async Task<ResponseDTO<IEnumerable<NotesEntity>>> RetriveNotes(int noteId, int userId)
        {
            try
            {
                var notes = _context.Notes.Where(x => x.UserId == userId && x.NoteId == noteId).ToList();
                if (notes != null)
                {
                    logger.Info($"Note with ID {noteId} retrieved for UserId: {userId}");
                    return new ResponseDTO<IEnumerable<NotesEntity>>()
                    {
                        success = true,
                        message = "Note Retrieved Successfully",
                        data = notes
                    };
                }
                else
                {
                    return new ResponseDTO<IEnumerable<NotesEntity>>()
                    {
                        success = false,
                        message = "Note Retrieval Failed",
                        data = null
                    };
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error occurred while retrieving specific note {noteId}");
                throw;
            }
        }

        public async Task<ResponseDTO<IEnumerable<NotesEntity>>> RetriveAllNotes(int userId)
        {
            try
            {
                string cacheKey = $"notes:accessible:user:{userId}";
                var cachedData = await _redisDb.StringGetAsync(cacheKey);

                if (cachedData.HasValue)
                {
                    var notes = JsonSerializer.Deserialize<List<NotesEntity>>(cachedData!);
                    logger.Info($"Notes retrieved from Redis for UserId: {userId}");
                    return new ResponseDTO<IEnumerable<NotesEntity>>()
                    {
                        success = true,
                        message = "Notes Retrieved Successfully (from cache)",
                        data = notes
                    };
                }

                // Get notes created by the user
                var ownNotes = _context.Notes.Where(n => n.UserId == userId);

                // Get notes shared with the user
                var sharedNoteIds = _context.NoteCollaborators
                    .Where(nc => nc.UserId == userId)
                    .Select(nc => nc.NoteId);

                var sharedNotes = _context.Notes.Where(n => sharedNoteIds.Contains(n.NoteId));

                // Combine both
                var accessibleNotes = ownNotes.Union(sharedNotes).ToList();

                // Cache results
                await _redisDb.StringSetAsync(cacheKey, JsonSerializer.Serialize(accessibleNotes), TimeSpan.FromMinutes(10));

                logger.Info($"Notes retrieved from DB and cached for UserId: {userId}");

                return new ResponseDTO<IEnumerable<NotesEntity>>()
                {
                    success = true,
                    message = "Notes Retrieved Successfully",
                    data = accessibleNotes
                };
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error occurred while retrieving notes for user {userId}");
                throw;
            }
        }

        public async Task<ResponseDTO<NotesEntity>> UpdateNote(int userId, int noteid, NotesDTO notes)
        {
            try
            {
                var note = _context.Notes.FirstOrDefault(x => x.NoteId == noteid);
                if (note != null)
                {
                    note.Title = notes.Title;
                    note.Description = notes.Description;
                    note.Reminder = null;
                    note.Backgroundcolor = notes.Backgroundcolor;
                    note.Image = null;
                    note.Pin = notes.Pin;
                    note.Created = DateTime.Now;
                    note.Edited = DateTime.Now;
                    note.Trash = notes.Trash;
                    note.Archieve = notes.Archieve;
                    _context.SaveChanges();

                    await _redisDb.KeyDeleteAsync($"notes:user:{userId}");
                    logger.Info($"Note updated for NoteId: {noteid}, UserId: {userId}");

                    return new ResponseDTO<NotesEntity>()
                    {
                        success = true,
                        message = "Note Updated Successfully",
                        data = note
                    };
                }
                else
                {
                    return new ResponseDTO<NotesEntity>()
                    {
                        success = false,
                        message = "Note Update Failed",
                        data = null
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error occurred while updating note {noteid}");
                throw;
            }
        }

        public async Task<ResponseDTO<bool>> DeleteNote(int userId, int noteId)
        {
            try
            {
                var note = _context.Notes.FirstOrDefault(x => x.UserId == userId && x.NoteId == noteId);
                if (note != null)
                {
                    _context.Notes.Remove(note);
                    _context.SaveChanges();

                    await _redisDb.KeyDeleteAsync($"notes:user:{userId}");
                    logger.Info($"Note deleted for NoteId: {noteId}, UserId: {userId}");

                    return new ResponseDTO<bool>()
                    {
                        success = true,
                        message = "Note Deleted Successfully",
                        data = true
                    };
                }
                else
                {
                    return new ResponseDTO<bool>()
                    {
                        success = false,
                        message = "Note Deletion Failed",
                        data = false
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error occurred while deleting note {noteId}");
                throw;
            }
        }

        public async Task<ResponseDTO<bool>> TrashNote(int noteId, int userId)
        {
            try
            {
                var note = _context.Notes.FirstOrDefault(x => x.UserId == userId && x.NoteId == noteId);
                if (note != null)
                {
                    note.Trash = true;
                    _context.SaveChanges();

                    await _redisDb.KeyDeleteAsync($"notes:user:{userId}");
                    logger.Info($"Note trashed for NoteId: {noteId}, UserId: {userId}");

                    return new ResponseDTO<bool>()
                    {
                        success = true,
                        message = "Note Trashed Successfully",
                        data = true
                    };
                }
                else
                {
                    return new ResponseDTO<bool>()
                    {
                        success = false,
                        message = "Note Trash Failed",
                        data = false
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error occurred while trashing note {noteId}");
                throw;
            }
        }

        public async Task<ResponseDTO<bool>> PinNote(int noteId, int userId)
        {
            try
            {
                var note = _context.Notes.FirstOrDefault(x => x.UserId == userId && x.NoteId == noteId);
                if (note != null)
                {
                    note.Pin = true;
                    _context.SaveChanges();

                    await _redisDb.KeyDeleteAsync($"notes:user:{userId}");
                    logger.Info($"Note pinned for NoteId: {noteId}, UserId: {userId}");

                    return new ResponseDTO<bool>()
                    {
                        success = true,
                        message = "Note Pinned Successfully",
                        data = true
                    };
                }
                else
                {
                    return new ResponseDTO<bool>()
                    {
                        success = false,
                        message = "Note Pin Failed",
                        data = false
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error occurred while pinning note {noteId}");
                throw;
            }
        }

        public async Task<ResponseDTO<bool>> ArchiveNote(int userId, int noteId)
        {
            try
            {
                var note = _context.Notes.FirstOrDefault(x => x.UserId == userId && x.NoteId == noteId);
                if (note != null)
                {
                    note.Archieve = true;
                    _context.SaveChanges();

                    await _redisDb.KeyDeleteAsync($"notes:user:{userId}");
                    logger.Info($"Note archived for NoteId: {noteId}, UserId: {userId}");

                    return new ResponseDTO<bool>()
                    {
                        success = true,
                        message = "Note Archived Successfully",
                        data = true
                    };
                }
                else
                {
                    return new ResponseDTO<bool>()
                    {
                        success = false,
                        message = "Note Archive Failed",
                        data = false
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error occurred while archiving note {noteId}");
                throw;
            }
        }

        public async Task<ResponseDTO<string>> BackgroundColorNote(int noteId, string backgroundColor)
        {
            try
            {
                var note = _context.Notes.FirstOrDefault(x => x.NoteId == noteId);
                if (note != null)
                {
                    note.Backgroundcolor = backgroundColor;
                    _context.SaveChanges();

                    await _redisDb.KeyDeleteAsync($"notes:user:{note.UserId}");
                    logger.Info($"Background color updated for NoteId: {noteId}");

                    return new ResponseDTO<string>()
                    {
                        success = true,
                        message = "Background Color Changed Successfully",
                        data = backgroundColor
                    };
                }
                else
                {
                    return new ResponseDTO<string>()
                    {
                        success = false,
                        message = "Background Color Change Failed",
                        data = null
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error occurred while changing background color of note {noteId}");
                throw;
            }
        }

        public async Task<ResponseDTO<string>> ImageNotes(IFormFile image, int noteId, int userId)
        {
            try
            {
                var note = _context.Notes.FirstOrDefault(x => x.UserId == userId && x.NoteId == noteId);
                if (note != null)
                {
                    note.Image = image.FileName;
                    _context.SaveChanges();

                    await _redisDb.KeyDeleteAsync($"notes:user:{userId}");
                    logger.Info($"Image added to NoteId: {noteId}, UserId: {userId}");

                    return new ResponseDTO<string>()
                    {
                        success = true,
                        message = "Image Added Successfully",
                        data = image.FileName
                    };
                }
                else
                {
                    return new ResponseDTO<string>()
                    {
                        success = false,
                        message = "Image Addition Failed",
                        data = null
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error occurred while adding image to note {noteId}");
                throw;
            }
        }

        public async Task<ResponseDTO<bool>> UnarchiveNote(int userId, int noteId)
        {
            try
            {
                var note = _context.Notes.FirstOrDefault(x => x.UserId == userId && x.NoteId == noteId);
                if (note != null)
                {
                    note.Archieve = false;
                    _context.SaveChanges();

                    await _redisDb.KeyDeleteAsync($"notes:user:{userId}");
                    logger.Info($"Note unarchived for NoteId: {noteId}, UserId: {userId}");

                    return new ResponseDTO<bool>()
                    {
                        success = true,
                        message = "Note Unarchived Successfully",
                        data = true
                    };
                }
                else
                {
                    return new ResponseDTO<bool>()
                    {
                        success = false,
                        message = "Note Unarchive Failed",
                        data = false
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error occurred while unarchiving note {noteId}");
                throw;
            }
        }

        public async Task<ResponseDTO<bool>> RestoreNote(int noteId, int userId)
        {
            try
            {
                var note = _context.Notes.FirstOrDefault(x => x.UserId == userId && x.NoteId == noteId);
                if (note != null)
                {
                    note.Trash = false;
                    _context.SaveChanges();

                    await _redisDb.KeyDeleteAsync($"notes:user:{userId}");
                    logger.Info($"Note restored for NoteId: {noteId}, UserId: {userId}");

                    return new ResponseDTO<bool>()
                    {
                        success = true,
                        message = "Note Restored Successfully",
                        data = true
                    };
                }
                else
                {
                    return new ResponseDTO<bool>()
                    {
                        success = false,
                        message = "Note Restore Failed",
                        data = false
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error occurred while restoring note {noteId}");
                throw;
            }
        }



    }
}