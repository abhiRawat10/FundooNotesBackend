using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BuisnessLayer.Interfaces;
using BuisnessLayer.InterFaces;
using ModelLayer.Entity;
using RepositoryLayer.DTO;
using RepositoryLayer.Interfaces;

namespace BuisnessLayer.Services
{
    public class LabelImplBL : ILabelBL
    {
        private readonly ILabelRL _labelRL;
        public LabelImplBL(ILabelRL labelRL)
        {
            _labelRL = labelRL;
        }

        public async Task<ResponseDTO<bool>> CreateLabel(string labelName, int userId)
        {
            try
            {
                var result = await _labelRL.CreateLabel(labelName, userId);
                if (result.success)
                {
                    return new ResponseDTO<bool>
                    {
                        success = true,
                        message = "Label Created Successfully",
                        data = result.data
                    };
                }
                else
                {
                    return new ResponseDTO<bool>
                    {
                        success = false,
                        message = "Label Creation Failed",
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseDTO<bool>> DeleteLabel(int labelId, int userId)
        {
            try
            {
                var result = await _labelRL.DeleteLabel(labelId, userId);
                if (result.success)
                {
                    return new ResponseDTO<bool>
                    {
                        success = true,
                        message = "Label Deleted Successfully",
                        data = result.data
                    };
                }
                else
                {
                    return new ResponseDTO<bool>
                    {
                        success = false,
                        message = "Label Deletion Failed",
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseDTO<bool>> UpdateLabel(int labelId, string labelName, int userId)
        {
            try
            {
                var result = await _labelRL.UpdateLabel(labelId, labelName, userId);
                if (result.success)
                {
                    return new ResponseDTO<bool>
                    {
                        success = true,
                        message = "Label Updated Successfully",
                        data = result.data
                    };
                }
                else
                {
                    return new ResponseDTO<bool>
                    {
                        success = false,
                        message = "Label Update Failed",
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseDTO<List<LabelEntity>>> GetAllLabels(int userId)
        {
            try
            {
                var result = await _labelRL.GetAllLabels(userId);
                if (result.success)
                {
                    return new ResponseDTO<List<LabelEntity>>
                    {
                        success = true,
                        message = "Labels Retrieved Successfully",
                        data = result.data
                    };
                }
                else
                {
                    return new ResponseDTO<List<LabelEntity>>
                    {
                        success = false,
                        message = "Label Retrieval Failed",
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseDTO<bool>> AddLabelToNote(int noteId, int labelId, int userId)
        {
            try
            {
                var result = await _labelRL.AddLabelToNote(noteId, labelId, userId);
                if (result.success)
                {
                    return new ResponseDTO<bool>
                    {
                        success = true,
                        message = "Label Added to Note Successfully",
                        data = result.data
                    };
                }
                else
                {
                    return new ResponseDTO<bool>
                    {
                        success = false,
                        message = "Label Addition to Note Failed",
                    };
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<ResponseDTO<bool>> RemoveLabelFromNote(int noteId, int labelId, int userId)
        {
            try
            {
                var result = await _labelRL.RemoveLabelFromNote(noteId, labelId, userId);
                if (result.success)
                {
                    return new ResponseDTO<bool>
                    {
                        success = true,
                        message = "Label Removed from Note Successfully",
                        data = result.data
                    };
                }
                else
                {
                    return new ResponseDTO<bool>
                    {
                        success = false,
                        message = "Label Removal from Note Failed",
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
