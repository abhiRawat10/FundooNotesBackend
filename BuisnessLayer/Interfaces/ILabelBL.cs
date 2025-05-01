using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Entity;
using RepositoryLayer.DTO;

namespace BuisnessLayer.Interfaces
{
    public interface ILabelBL
    {
        Task<ResponseDTO<bool>> CreateLabel(string labelName, int userId);
        Task<ResponseDTO<bool>> DeleteLabel(int labelId, int userId);
        Task<ResponseDTO<bool>> UpdateLabel(int labelId, string labelName, int userId);
        Task<ResponseDTO<List<LabelEntity>>> GetAllLabels(int userId);
        Task<ResponseDTO<bool>> AddLabelToNote(int noteId, int labelId, int userId);
        Task<ResponseDTO<bool>> RemoveLabelFromNote(int noteId, int labelId, int userId);

    }
}
