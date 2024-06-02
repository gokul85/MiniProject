using ReturnManagementSystem.Models;
using ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs;

namespace ReturnManagementSystem.Interfaces
{
    public interface IReturnRequestService
    {
        Task<ReturnRequest> OpenReturnRequest(ReturnRequestDTO returnRequestDTO);
        Task<ReturnRequest> UpdateUserSerialNumber(UpdateRequestSerialNumberDTO ursnDTO);
        Task<ReturnRequest> TechnicalReview(TechnicalReviewDTO technicalReviewDTO);
        Task<ReturnRequest> CloseReturnRequest( int userId, CloseRequestDTO crrDTO);
        Task<IEnumerable<ReturnRequest>> GetAllReturnRequests();
        Task<IEnumerable<ReturnRequest>> GetAllUserReturnRequests(int userid);
        Task<ReturnRequest> GetReturnRequest(int requestid);
    }

}
