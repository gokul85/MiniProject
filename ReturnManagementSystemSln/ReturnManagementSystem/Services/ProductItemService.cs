using ReturnManagementSystem.Exceptions;
using ReturnManagementSystem.Interfaces;
using ReturnManagementSystem.Models;
using ReturnManagementSystem.Models.DTOs.ProductDTOs;
using ReturnManagementSystem.Repositories;

namespace ReturnManagementSystem.Services
{
    public class ProductItemService : IProductItemService
    {
        private readonly IRepository<int,ProductItem> _productItemRepository;
        private readonly IRepository<int,Product> _productRepository;

        public ProductItemService(IRepository<int, ProductItem> productItemRepository, IRepository<int, Product> productRepository)
        {
            _productItemRepository = productItemRepository;
            _productRepository = productRepository;
        }
        public async Task<AddProductItemsResponse> AddProductItem(List<AddProductItemDTO> addpiDTO)
        {
            AddProductItemsResponse pires = new AddProductItemsResponse();
            foreach(var pi in  addpiDTO)
            {
                var res = await _productRepository.Get(pi.ProductId);
                if(res == null)
                {
                    if (pires.ProductIdsNotFound.Contains(pi.ProductId))
                        continue;
                    pires.ProductIdsNotFound.Add(pi.ProductId);
                    continue;
                }
                var res2 = await _productItemRepository.FindAll(pis => pis.SerialNumber == pi.SerialNumber && pis.ProductId == pi.ProductId);
                if(res2 != null)
                {
                    if (pires.SerialNumbersExists.Contains(pi.SerialNumber))
                        continue;
                    pires.SerialNumbersExists.Add(pi.SerialNumber);
                    continue;
                }
                var productItem = new ProductItem
                {
                    SerialNumber = pi.SerialNumber,
                    Status = "Available",
                    ProductId = pi.ProductId,
                };
                var productitem = await _productItemRepository.Add(productItem);
                var product = await _productRepository.Get(pi.ProductId);
                product.Stock += 1;
                await _productRepository.Update(product);
                pires.AddedProductItems.Add(productitem);
            }
            return pires;
        }

        public async Task<IEnumerable<ProductItem>> GetAllProductItems()
        {
            var productitems = await _productItemRepository.GetAll();
            if (productitems != null)
                return productitems;
            throw new ObjectsNotFoundException("Product Items Not Found");
        }

        public async Task<IEnumerable<ProductItem>> GetAllProductItems(string status)
        {
            var productitems = await _productItemRepository.FindAll(pi => pi.Status == status);
            if (productitems != null)
                return productitems;
            throw new ObjectsNotFoundException($"Product Items Not Found with status {status}");
        }

        public async Task<IEnumerable<ProductItem>> GetAllProductItemsByProductId(int productid)
        {
            var productitems = await _productItemRepository.FindAll(pi => pi.ProductId == productid);
            if (productitems != null)
                return productitems;
            throw new ObjectsNotFoundException($"Product Items Not Found with product id {productid}");
        }

        public async Task<ProductItem> UpdateProductItemRefurbished(int productId, string serialNumber)
        {
            var productItems = await _productItemRepository.FindAll(pi => pi.SerialNumber == serialNumber);
            if (productItems == null)
            {
                throw new ObjectNotFoundException("Product item not found");
            }
            var productItem = productItems.FirstOrDefault();
            productItem.ProductId = productId;
            productItem.Status = "Refurbished";
            var product = await _productRepository.Get(productItem.ProductId ?? 0);
            if(product == null)
            {
                throw new ObjectNotFoundException("Product Not Found");
            }
            product.Stock += 1;
            await _productRepository.Update(product);

            return await _productItemRepository.Update(productItem);
        }

        public async Task<ProductItem> UpdateProductItemStatus(UpdateProductItemStatus upisDTO)
        {
            var productItems = await _productItemRepository.FindAll(pi => pi.SerialNumber == upisDTO.SerialNumber);
            if (productItems == null)
            {
                throw new ObjectNotFoundException("Product item not found");
            }
            var productItem = productItems.FirstOrDefault();
            if (productItem.Status == upisDTO.Status)
            {
                return productItem;
            }
            if(upisDTO.Status == "Available")
            {
                var product = await _productRepository.Get(productItem.ProductId??0);
                product.Stock += 1;
                await _productRepository.Update(product);
            }
            else if (productItem.Status == "Available")
            {
                var product = await _productRepository.Get(productItem.ProductId ?? 0);
                product.Stock -= 1;
                await _productRepository.Update(product);
            }
            productItem.Status = upisDTO.Status;

            return await _productItemRepository.Update(productItem);
        }
    }
}
