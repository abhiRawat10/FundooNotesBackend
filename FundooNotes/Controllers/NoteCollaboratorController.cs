using BuisnessLayer.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace WebApplication1.Controllers
{
    public class NoteCollaboratorController: ControllerBase
    {
        public readonly INoteCollaboratorBL _noteCollaboratorBL;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public NoteCollaboratorController(INoteCollaboratorBL noteCollaboratorBL)
        {
            this._noteCollaboratorBL = noteCollaboratorBL;
        }

        [Authorize]
        [HttpPost("shareNote")]
        public async Task<IActionResult> ShareNote(int noteId, string collaboratorEmail)
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            try
            {
                logger.Info("ShareNote called with noteId: {0}, collaboratorEmail: {1}", noteId, collaboratorEmail);
                var result = await _noteCollaboratorBL.ShareNote(noteId, collaboratorEmail);
                if (result != null)
                {
                    logger.Info("Note shared successfully for noteId: {0}, collaboratorEmail: {1}", noteId, collaboratorEmail);
                    return Ok(result);
                }
                logger.Warn("Note sharing failed for noteId: {0}, collaboratorEmail: {1}", noteId, collaboratorEmail);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in ShareNote for noteId: {0}, collaboratorEmail: {1}", noteId, collaboratorEmail);
                return StatusCode(500, new { message = ex.Message });
            }

        }
    }
}
