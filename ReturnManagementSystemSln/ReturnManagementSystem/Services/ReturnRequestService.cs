using ReturnManagementSystem.Exceptions;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models;
using ReturnManagementSystem.Models.DTOs.ProductDTOs;
using ReturnManagementSystem.Models.DTOs.RRandPaymentDTOs;
using ReturnManagementSystem.Repositories;

namespace ReturnManagementSystem.Services
{
    public class ReturnRequestService : IReturnRequestService
    {
        private readonly IRepository<int,ReturnRequest> _returnRequestRepository;
        private readonly IRepository<int,Order> _orderRepository;
        private readonly IRepository<int,OrderProduct> _orderProductRepository;
        private readonly IRepository<int,Product> _productRepository;
        private readonly IRepository<int,ProductItem> _productItemRepository;
        private readonly IRepository<int,Policy> _policyRepository;
        IProductItemService _productItemService;

        public ReturnRequestService(IRepository<int, ReturnRequest> returnRequestRepository,
            IRepository<int, Order> orderRepository,
            IRepository<int, OrderProduct> orderProductRepository,
            IRepository<int, Product> productRepository,
            IRepository<int, ProductItem> productItemRepository,
            IRepository<int, Policy> policyRepository,
            IProductItemService productItemService
            )
        {
            _returnRequestRepository = returnRequestRepository;
            _orderProductRepository = orderProductRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _productItemRepository = productItemRepository;
            _policyRepository = policyRepository;
            _productItemService = productItemService;
        }
        public async Task<ReturnRequest> CloseReturnRequest(int requestId, int userId ,string feedback)
        {
            var returnRequest = await _returnRequestRepository.Get(requestId);
            if (returnRequest == null)
            {
                throw new ObjectNotFoundException("Return request not found");
            }

            returnRequest.Feedback = feedback;
            returnRequest.Status = "Closed";
            returnRequest.ClosedDate = DateTime.UtcNow;
            returnRequest.ClosedBy = userId;

            return await _returnRequestRepository.Update(returnRequest);
        }

        public async Task<ReturnRequest> OpenReturnRequest(ReturnRequestDTO returnRequestDTO)
        {
            var order = await _orderRepository.Get(returnRequestDTO.OrderId);
            if( order == null || order.UserId != returnRequestDTO.UserId || order.OrderStatus != "Delivered")
            {
                throw new ObjectNotFoundException("Invalid Order");
            }
            var opress = order.OrderProducts.Where(op => op.ProductId == returnRequestDTO.ProductId).FirstOrDefault();
            if(opress == null)
            {
                throw new ObjectNotFoundException("Product Not Found in the Order");
            }
            var policyress = await _policyRepository.FindAll(pr => pr.ProductId == returnRequestDTO.ProductId && pr.PolicyType == returnRequestDTO.ReturnPolicy);
            if(policyress == null)
            {
                throw new InvalidReturnRequest("Specified Return Policy is not Applicable for this Product");
            }
            var policyres = policyress.FirstOrDefault();
            var orderedDate = order.OrderDate;
            var duration = (DateTime.Now - orderedDate).Value.TotalDays;
            if(duration > policyres.Duration)
            {
                throw new InvalidReturnRequest("The Return Policy Duration has been Exceeded");
            }
            var returnRequest = new ReturnRequest
            {
                UserId = returnRequestDTO.UserId,
                OrderId = returnRequestDTO.OrderId,
                ProductId = returnRequestDTO.ProductId,
                ReturnPolicy = returnRequestDTO.ReturnPolicy,
                Reason = returnRequestDTO.Reason,
                RequestDate = DateTime.Now,
                Status = "Pending"
            };
            return await _returnRequestRepository.Add(returnRequest);
        }

        public async Task<ReturnRequest> TechnicalReview(int requestId, string process, string feedback)
        {
            var returnRequest = await _returnRequestRepository.Get(requestId);
            if (returnRequest == null)
            {
                throw new ObjectNotFoundException("Return request not found");
            }

            if(process == "Return Good" || process == "Return Bad" || process == "Replace Repaired" || process == "Replace Bad" || process == "Repaired")
            {
                if (process == "Return Good" || process == "Return Bad")
                {
                    //Transaction need to done here
                    if (process == "Return Bad")
                    {
                        await _productItemService.UpdateProductItemStatus(returnRequest.SerialNumber, "Dispossed");
                    }
                    if (process == "Return Good")
                    {
                        await _productItemService.UpdateProductItemStatus(returnRequest.SerialNumber, "Available");
                    }
                }
                else if (process == "Replace Repaired" || process == "Replace Bad")
                {
                    var asns = await _productItemRepository.FindAll(pi => pi.ProductId == returnRequest.ProductId && pi.Status == "Available");
                    if (asns == null)
                    {
                        throw new InvalidDataException("Product Out of Stock for Replacement");
                    }
                    var asn = asns.FirstOrDefault();
                    asn.Status = "Replaced";
                    await _productItemRepository.Update(asn);
                    var orderproducts = await _orderProductRepository.FindAll(op => op.OrderId == returnRequest.OrderId && op.ProductId == returnRequest.ProductId && op.SerialNumber == returnRequest.SerialNumber);
                    var orderproduct = orderproducts.FirstOrDefault();
                    orderproduct.SerialNumber = asn.SerialNumber;
                    await _orderProductRepository.Update(orderproduct);
                    if (process == "Replace Bad")
                    {
                        await _productItemService.UpdateProductItemStatus(returnRequest.SerialNumber, "Dispossed");
                    }
                    else
                    {
                        var prods = await _productRepository.FindAll(p => p.Name == returnRequest.Product.Name && p.ProductStatus == "Refurbished");
                        if (prods == null)
                        {
                            var pro = await _productRepository.Get((int)returnRequest.ProductId);
                            Product newprod = new Product
                            {
                                Name = pro.Name,
                                Description = pro.Description,
                                Price = Math.Round((decimal)(pro.Price * 0.75m), 0),
                                Stock = 0,
                                ProductStatus = "Refurbished",
                            };
                            var prod = await _productRepository.Add(newprod);
                            await _productItemService.UpdateProductItemRefurbished(prod.ProductId, returnRequest.SerialNumber);
                        }
                        else
                        {
                            var prod = prods.FirstOrDefault();
                            await _productItemService.UpdateProductItemRefurbished(prod.ProductId, returnRequest.SerialNumber);
                        }
                    }
                }
                else if (process == "Repaired")
                {
                    await _productItemService.UpdateProductItemStatus(returnRequest.SerialNumber, "Repaired");
                }

                returnRequest.Process = process;
                returnRequest.Feedback = feedback;

                return await _returnRequestRepository.Update(returnRequest);
            }
            
            throw new InvalidDataException("Invalid Process");

        }

        public async Task<ReturnRequest> UpdateUserSerialNumber(int requestId, string serialNumber)
        {
            var returnRequest = await _returnRequestRepository.Get(requestId);
            if (returnRequest == null)
            {
                throw new ObjectNotFoundException("Return request not found");
            }
            var sn = await _orderProductRepository.FindAll(op=>op.OrderId == returnRequest.OrderId && op.SerialNumber == serialNumber);
            if(sn == null)
            {
                throw new InvalidSerialNumber("Invalid Serial Number");
            }
            returnRequest.SerialNumber = serialNumber;
            returnRequest.Status = "Processing";

            return await _returnRequestRepository.Update(returnRequest);
        }
    }
}
