using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Entity;
using NLog;
using RepositoryLayer.Context;
using RepositoryLayer.DTO;
using RepositoryLayer.Interfaces;
using StackExchange.Redis;

namespace RepositoryLayer.Services
{
    public class LabelImplRL : ILabelRL
    {
        private readonly UserContext _context;
        private readonly IDatabase _redisDb;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public LabelImplRL(UserContext context, IDatabase redisDb)
        {
            _context = context;
            _redisDb = redisDb;
        }

        public async Task<ResponseDTO<bool>> CreateLabel(string labelName, int userId)
        {
            try
            {
                if (string.IsNullOrEmpty(labelName))
                {
                    return new ResponseDTO<bool>()
                    {
                        success = false,
                        message = "Label name cannot be empty",
                        data = false
                    };
                }
                // Check if label already exists for the user
                var existingLabel = _context.Labels.FirstOrDefault(x => x.Name == labelName && x.UserId == userId);
                if (existingLabel != null)
                {
                    return new ResponseDTO<bool>()
                    {
                        success = false,
                        message = "Label already exists",
                        data = false
                    };
                }
                LabelEntity label = new LabelEntity()
                {
                    Name = labelName,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Labels.Add(label);
                _context.SaveChanges();
                await _redisDb.KeyDeleteAsync($"labels:user:{userId}");
                logger.Info($"Label created for UserId: {userId}");
                return new ResponseDTO<bool>()
                {
                    success = true,
                    message = "Label Created Successfully",
                    data = true
                };
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error occurred while creating label for user {userId}");
                throw;

            }
        }

        public async Task<ResponseDTO<bool>> DeleteLabel(int labelId, int userId)
        {
            try
            {
                var label = _context.Labels.FirstOrDefault(x => x.Id == labelId && x.UserId == userId);
                if (label != null)
                {
                    _context.Labels.Remove(label);
                    _context.SaveChanges();
                    await _redisDb.KeyDeleteAsync($"labels:user:{userId}");
                    logger.Info($"Label deleted for LabelId: {labelId}, UserId: {userId}");
                    return new ResponseDTO<bool>()
                    {
                        success = true,
                        message = "Label Deleted Successfully",
                        data = true
                    };
                }
                else
                {
                    return new ResponseDTO<bool>()
                    {
                        success = false,
                        message = "Label Deletion Failed",
                        data = false
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error occurred while deleting label {labelId}");
                throw;
            }
        }

        public async Task<ResponseDTO<bool>> UpdateLabel(int labelId, string labelName, int userId)
        {
            try
            {
                var label = _context.Labels.FirstOrDefault(x => x.Id == labelId && x.UserId == userId);
                if (label != null)
                {
                    label.Name = labelName;
                    _context.SaveChanges();
                    await _redisDb.KeyDeleteAsync($"labels:user:{userId}");
                    logger.Info($"Label updated for LabelId: {labelId}, UserId: {userId}");
                    return new ResponseDTO<bool>()
                    {
                        success = true,
                        message = "Label Updated Successfully",
                        data = true
                    };
                }
                else
                {
                    return new ResponseDTO<bool>()
                    {
                        success = false,
                        message = "Label Update Failed",
                        data = false
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error occurred while updating label {labelId}");
                throw;
            }
        }

        public async Task<ResponseDTO<List<LabelEntity>>> GetAllLabels(int userId)
        {
            try
            {
                var labels = await _redisDb.StringGetAsync($"labels:user:{userId}");
                if (labels.IsNullOrEmpty)
                {
                    var labelList = _context.Labels.Where(x => x.UserId == userId).ToList(); //getting all labels of a user
                    if (labelList != null)
                    {
                        await _redisDb.StringSetAsync($"labels:user:{userId}", labelList.ToString());
                        return new ResponseDTO<List<LabelEntity>>()
                        {
                            success = true,
                            message = "Labels Retrieved Successfully",
                            data = labelList
                        };
                    }
                    else
                    {
                        return new ResponseDTO<List<LabelEntity>>()
                        {
                            success = false,
                            message = "No Labels Found",
                            data = null
                        };
                    }
                }
                else
                {
                    return new ResponseDTO<List<LabelEntity>>()
                    {
                        success = true,
                        message = "Labels Retrieved Successfully from Cache",
                        data = null
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"Error occurred while retrieving labels for user {userId}");
                throw;
            }
        }

        public async Task<ResponseDTO<bool>> AddLabelToNote(int labelId, int noteId, int userId)
        {
            try
            {
                var label = _context.Labels.FirstOrDefault(x => x.Id == labelId && x.UserId == userId);
                if (label != null)
                {
                    var note = _context.Notes.FirstOrDefault(x => x.NoteId == noteId);
                    if (note != null)
                    {
                        label.Notes.Add(note);
                        return new ResponseDTO<bool>()
                        {
                            success = true,
                            message = "message",
                            data = true
                        };
                    }
                    return new ResponseDTO<bool>()
                    {
                        success = false,
                        message = "note doesnt exist",
                        data = false
                    };
                }
                return new ResponseDTO<bool>()
                {
                    success = false,
                    message = "adding to label failed as label do not exist or belong to the current user",
                    data = false
                };

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public async Task<ResponseDTO<bool>> RemoveLabelFromNote(int labelId, int noteId, int userId)
        {
            try
            {
                var label = _context.Labels.FirstOrDefault(x => x.Id == labelId && x.UserId == userId);
                if (label != null)
                {
                    var note = _context.Notes.FirstOrDefault(x => x.NoteId == noteId);
                    if (note != null)
                    {
                        label.Notes.Remove(note);
                        return new ResponseDTO<bool>()
                        {
                            success = true,
                            message = "message",
                            data = true
                        };
                    }
                    return new ResponseDTO<bool>()
                    {
                        success = false,
                        message = "note doesnt exist",
                        data = false
                    };
                }
                return new ResponseDTO<bool>()
                {
                    success = false,
                    message = "removing from label failed as label do not exist or belong to the current user",
                    data = false
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
