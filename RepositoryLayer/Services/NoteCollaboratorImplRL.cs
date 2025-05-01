using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using ModelLayer.Entity;
using RepositoryLayer.Context;
using RepositoryLayer.DTO;
using NLog;
using RepositoryLayer.Interfaces;

namespace RepositoryLayer.Services
{
    public class NoteCollaboratorImplRL : INoteCollaboratorRL
    {
        private readonly UserContext _context;
        private readonly IDatabase _redisDb;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public NoteCollaboratorImplRL(UserContext context, IDatabase redisDb)
        {
            _context = context;
            _redisDb = redisDb;
        }
        public async Task<ResponseDTO<bool>> ShareNote(int noteId, string collaboratorEmail)
        {
            try
            {
                var note = _context.Notes
                    .Include(n => n.Collaborators)
                    .FirstOrDefault(x => x.NoteId == noteId);

                if (note == null)
                {
                    return new ResponseDTO<bool>()
                    {
                        success = false,
                        message = "Note Not Found",
                        data = false
                    };
                }

                var collaborator = _context.Users.FirstOrDefault(x => x.Email == collaboratorEmail);
                if (collaborator == null)
                {
                    return new ResponseDTO<bool>()
                    {
                        success = false,
                        message = "Collaborator Not Found",
                        data = false
                    };
                }

                // Check if already shared
                bool alreadyShared = note.Collaborators.Any(nc => nc.UserId == collaborator.Id);
                if (alreadyShared)
                {
                    return new ResponseDTO<bool>()
                    {
                        success = false,
                        message = "Note already shared with this user",
                        data = false
                    };
                }

                // Create and add the collaborator
                NoteCollaboratorEntity noteCollaborator = new NoteCollaboratorEntity()
                {
                    NoteId = noteId,
                    UserId = collaborator.Id
                };

                // Add to both EF context and navigation property
                _context.NoteCollaborators.Add(noteCollaborator);
                note.Collaborators.Add(noteCollaborator);

                _context.SaveChanges();

                // Invalidate Redis cache for both owner and collaborator
                await _redisDb.KeyDeleteAsync($"notes:accessible:user:{note.UserId}");
                await _redisDb.KeyDeleteAsync($"notes:accessible:user:{collaborator.Id}");

                logger.Info($"NoteId {noteId} shared with {collaboratorEmail} (UserId: {collaborator.Id})");

                return new ResponseDTO<bool>()
                {
                    success = true,
                    message = "Note Shared Successfully",
                    data = true
                };
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error occurred while sharing note {noteId}");
                throw;
            }
        }
    }
}
