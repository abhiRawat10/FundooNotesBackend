using BuisnessLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NLog;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabelController : ControllerBase
    {
        private readonly ILabelBL _labelBL;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public LabelController(ILabelBL labelBL)
        {
            _labelBL = labelBL;
        }

        [HttpPost("CreateLabel")]
        public async Task<IActionResult> CreateLabel(string labelName)
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            try
            {
                logger.Info("CreateLabel called with labelName: {0}, userId: {1}", labelName, userId);
                var result = await _labelBL.CreateLabel(labelName, userId);
                if (result != null)
                {
                    logger.Info("Label created successfully for labelName: {0}, userId: {1}", labelName, userId);
                    return Ok(result);
                }
                logger.Warn("Label creation failed for labelName: {0}, userId: {1}", labelName, userId);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in CreateLabel for labelName: {0}, userId: {1}", labelName, userId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("DeleteLabel")]
        public async Task<IActionResult> DeleteLabel(int labelId)
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            try
            {
                logger.Info("DeleteLabel called with labelId: {0}, userId: {1}", labelId, userId);
                var result = await _labelBL.DeleteLabel(labelId, userId);
                if (result != null)
                {
                    logger.Info("Label deleted successfully for labelId: {0}, userId: {1}", labelId, userId);
                    return Ok(result);
                }
                logger.Warn("Label deletion failed for labelId: {0}, userId: {1}", labelId, userId);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in DeleteLabel for labelId: {0}, userId: {1}", labelId, userId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("UpdateLabel")]
        public async Task<IActionResult> UpdateLabel(int labelId, string labelName)
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            try
            {
                logger.Info("UpdateLabel called with labelId: {0}, labelName: {1}, userId: {2}", labelId, labelName, userId);
                var result = await _labelBL.UpdateLabel(labelId, labelName, userId);
                if (result != null)
                {
                    logger.Info("Label updated successfully for labelId: {0}, labelName: {1}, userId: {2}", labelId, labelName, userId);
                    return Ok(result);
                }
                logger.Warn("Label update failed for labelId: {0}, labelName: {1}, userId: {2}", labelId, labelName, userId);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in UpdateLabel for labelId: {0}, labelName: {1}, userId: {2}", labelId, labelName, userId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("GetAllLabels")]
        public async Task<IActionResult> GetAllLabels()
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            try
            {
                logger.Info("GetAllLabels called for userId: {0}", userId);
                var result = await _labelBL.GetAllLabels(userId);
                if (result != null)
                {
                    logger.Info("Labels retrieved successfully for userId: {0}", userId);
                    return Ok(result);
                }
                logger.Warn("No labels found for userId: {0}", userId);
                return NotFound(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in GetAllLabels for userId: {0}", userId);
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("AddLabelToNote")]
        public async Task<IActionResult> AddLabelToNote(int noteId, int labelId)
        {
            var userId = Convert.ToInt32(User.FindFirst("UserId")?.Value);
            try
            {
                logger.Info("AddLabelToNote called with noteId: {0}, labelId: {1}, userId: {2}", noteId, labelId, userId);
                var result = await _labelBL.AddLabelToNote(noteId, labelId, userId);
                if (result != null)
                {
                    logger.Info("Label added to note successfully for noteId: {0}, labelId: {1}, userId: {2}", noteId, labelId, userId);
                    return Ok(result);
                }
                logger.Warn("Adding label to note failed for noteId: {0}, labelId: {1}, userId: {2}", noteId, labelId, userId);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Exception in AddLabelToNote for noteId: {0}, labelId: {1}, userId: {2}", noteId, labelId, userId);
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
