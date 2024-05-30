using ReturnManagementSystem.Models;
using ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs;

namespace ReturnManagementSystem.Interfaces
{
    public interface IReturnRequestService
    {
        Task<ReturnRequest> OpenReturnRequest(ReturnRequestDTO returnRequestDTO);
        Task<ReturnRequest> UpdateUserSerialNumber(int requestId, string serialNumber);
        Task<ReturnRequest> TechnicalReview(int requestId, string process, string feedback);
        Task<ReturnRequest> CloseReturnRequest(int requestId, int userId, string feedback);
    }

}
