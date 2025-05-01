using BuisnessLayer.InterFaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;
using RepositoryLayer.DTO;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotesController : ControllerBase
    {
        //private readonly IUserBL _userBL;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly INotesBL _notesBL;

        public NotesController(IUserBL userBL, INotesBL notesBL)
        {
            //_userBL = userBL;
            _notesBL = notesBL;
        }

        [Authorize]
        [HttpPost("CreateNote")]
        public async Task<IActionResult> CreateNote(NotesDTO note)
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            if (userId == null)
            {
                logger.Warn("UserId not found in token");
                return BadRequest(new { message = "UserId not found in token" });
            }
            try
            {
                if (note == null)
                {
                    logger.Warn("Note object is null");
                    return BadRequest(new { message = "Note object is null" });
                }
                logger.Info("CreateNote called with userId: {0}", userId);
                var result = await _notesBL.CreateNotes(note, userId);
                if (result != null)
                {
                    logger.Info("Note created successfully for userId: {0}", userId);
                    return Ok(result);
                }
                logger.Warn("Note creation failed for userId: {0}", userId);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in CreateNote for userId: {0}", userId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("GetNote")]
        public async Task<IActionResult> GetNote(int noteId)
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            try
            {
                logger.Info("GetNote called with noteId: {0}, userId: {1}", noteId, userId);
                var result = await _notesBL.RetriveNotes(noteId, userId);
                if (result != null)
                {
                    logger.Info("Note retrieved successfully for noteId: {0}, userId: {1}", noteId, userId);
                    return Ok(result);
                }
                logger.Warn("Note retrieval failed for noteId: {0}, userId: {1}", noteId, userId);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in GetNote for noteId: {0}, userId: {1}", noteId, userId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("RetreiveAllNotes")]
        public async Task<IActionResult> RetreivAlleNotes()
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            try
            {
                logger.Info("GetAllNotes called with userId: {0}", userId);
                var result = await _notesBL.RetriveAllNotes(userId);
                if (result != null)
                {
                    logger.Info("All notes retrieved successfully for userId: {0}", userId);
                    return Ok(result);
                }
                logger.Warn("All notes retrieval failed for userId: {0}", userId);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in GetAllNotes for userId: {0}", userId);
                return StatusCode(500, new { message = ex.Message });
            }

        }

        [Authorize]
        [HttpPut("UpdateNote")]
        public async Task<IActionResult> UpdateNote(int noteId, NotesDTO notes)
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            try
            {
                logger.Info("UpdateNote called with userId: {0}, noteId: {1}", userId, noteId);
                var result = await _notesBL.UpdateNote(userId, noteId, notes);
                if (result != null)
                {
                    logger.Info("Note updated successfully for userId: {0}, noteId: {1}", userId, noteId);
                    return Ok(result);
                }
                logger.Warn("Note update failed for userId: {0}, noteId: {1}", userId, noteId);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in UpdateNote for userId: {0}, noteId: {1}", userId, noteId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("DeleteNote")]
        public async Task<IActionResult> DeleteNote(int noteId)
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            try
            {
                logger.Info("DeleteNote called with userId: {0}, noteId: {1}", userId, noteId);
                var result = await _notesBL.DeleteNote(userId, noteId);
                if (result != null)
                {
                    logger.Info("Note deleted successfully for userId: {0}, noteId: {1}", userId, noteId);
                    return Ok(result);
                }
                logger.Warn("Note deletion failed for userId: {0}, noteId: {1}", userId, noteId);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in DeleteNote for userId: {0}, noteId: {1}", userId, noteId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("TrashNote")]
        public async Task<IActionResult> TrashNote(int noteId)
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            try
            {
                logger.Info("TrashNote called with noteId: {0}, userId: {1}", noteId, userId);
                var result = await _notesBL.TrashNote(noteId, userId);
                if (result != null)
                {
                    logger.Info("Note trashed successfully for noteId: {0}, userId: {1}", noteId, userId);
                    return Ok(result);
                }
                logger.Warn("Note trashing failed for noteId: {0}, userId: {1}", noteId, userId);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in TrashNote for noteId: {0}, userId: {1}", noteId, userId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("PinNote")]
        public async Task<IActionResult> PinNote(int noteId)
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            try
            {
                logger.Info("PinNote called with noteId: {0}, userId: {1}", noteId, userId);
                var result = await _notesBL.PinNote(noteId, userId);
                if (result != null)
                {
                    logger.Info("Note pinned successfully for noteId: {0}, userId: {1}", noteId, userId);
                    return Ok(result);
                }
                logger.Warn("Note pinning failed for noteId: {0}, userId: {1}", noteId, userId);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in PinNote for noteId: {0}, userId: {1}", noteId, userId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("ArchiveNote")]
        public async Task<IActionResult> ArchiveNote(int noteId)
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            try
            {
                logger.Info("ArchiveNote called with userId: {0}, noteId: {1}", userId, noteId);
                var result = await _notesBL.ArchiveNote(userId, noteId);
                if (result != null)
                {
                    logger.Info("Note archived successfully for userId: {0}, noteId: {1}", userId, noteId);
                    return Ok(result);
                }
                logger.Warn("Note archiving failed for userId: {0}, noteId: {1}", userId, noteId);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in ArchiveNote for userId: {0}, noteId: {1}", userId, noteId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("UnarchiveNote")]
        public async Task<IActionResult> UnarchiveNote(int noteId)
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            try
            {
                logger.Info("UnarchiveNote called with userId: {0}, noteId: {1}", userId, noteId);
                var result = await _notesBL.UnarchiveNote(userId, noteId);
                if (result != null)
                {
                    logger.Info("Note unarchived successfully for userId: {0}, noteId: {1}", userId, noteId);
                    return Ok(result);
                }
                logger.Warn("Note unarchiving failed for userId: {0}, noteId: {1}", userId, noteId);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in UnarchiveNote for userId: {0}, noteId: {1}", userId, noteId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("RestoreNote")]
        public async Task<IActionResult> RestoreNote(int noteId)
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            try
            {
                logger.Info("RestoreNote called with noteId: {0}, userId: {1}", noteId, userId);
                var result = await _notesBL.RestoreNote(noteId, userId);
                if (result != null)
                {
                    logger.Info("Note restored successfully for noteId: {0}, userId: {1}", noteId, userId);
                    return Ok(result);
                }
                logger.Warn("Note restoration failed for noteId: {0}, userId: {1}", noteId, userId);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in RestoreNote for noteId: {0}, userId: {1}", noteId, userId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("BackgroundColorNote")]
        public async Task<IActionResult> BackgroundColorNote(int noteId, string backgroundColor)
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            try
            {
                logger.Info("BackgroundColorNote called with noteId: {0}, backgroundColor: {1}", noteId, backgroundColor);
                var result = await _notesBL.BackgroundColorNote(noteId, backgroundColor);
                if (result != null)
                {
                    logger.Info("Background color changed successfully for noteId: {0}", noteId);
                    return Ok(result);
                }
                logger.Warn("Background color change failed for noteId: {0}", noteId);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in BackgroundColorNote for noteId: {0}", noteId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("ImageNotes")]
        public async Task<IActionResult> ImageNotes(IFormFile image, int noteId)
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            if (image == null || image.Length == 0)
            {
                logger.Warn("Image file is null or empty");
                return BadRequest(new { message = "Image file is null or empty" });
            }
            try
            {
                logger.Info("ImageNotes called with noteId: {0}, userId: {1}", noteId, userId);
                var result = await _notesBL.ImageNotes(image, noteId, userId);
                if (result != null)
                {
                    logger.Info("Image added successfully for noteId: {0}, userId: {1}", noteId, userId);
                    return Ok(result);
                }
                logger.Warn("Image addition failed for noteId: {0}, userId: {1}", noteId, userId);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in ImageNotes for noteId: {0}, userId: {1}", noteId, userId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        
    }   
}
