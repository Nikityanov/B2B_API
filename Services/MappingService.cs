using B2B_API.Models;
using B2B_API.Models.DTO;

namespace B2B_API.Services
{    
    public static class MappingService
    {
        public static Product ToEntity(this ProductCreateDto dto)
        {
            return new Product
            {
                Name = dto.Name,
                Description = dto.Description ?? string.Empty,
                StockQuantity = dto.StockQuantity,
                Price = dto.Price,
                SKU = dto.SKU,
                Manufacturer = dto.Manufacturer ?? string.Empty,
                Unit = dto.Unit ?? string.Empty,
                ImageUrl = dto.ImageUrl ?? string.Empty,
                ImageGallery = dto.ImageGallery ?? new List<string>(),
                Characteristics = dto.Characteristics,
                CategoryId = dto.CategoryId
            };
        }

        public static void UpdateFromDto(this Product entity, ProductUpdateDto dto)
        {
            entity.Name = dto.Name;
            entity.Description = dto.Description ?? string.Empty;
            entity.StockQuantity = dto.StockQuantity;
            entity.Price = dto.Price;
            entity.Manufacturer = dto.Manufacturer ?? string.Empty;
            entity.Unit = dto.Unit ?? string.Empty;
            entity.ImageUrl = dto.ImageUrl ?? string.Empty;
            entity.ImageGallery = dto.ImageGallery ?? new List<string>();
            entity.Characteristics = dto.Characteristics;
            entity.CategoryId = dto.CategoryId;
        }

        public static ProductResponseDto ToDto(this Product entity)
        {
            return new ProductResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description ?? string.Empty,
                StockQuantity = entity.StockQuantity,
                Price = entity.Price,
                SKU = entity.SKU,
                Manufacturer = entity.Manufacturer,
                Unit = entity.Unit,
                ImageUrl = entity.ImageUrl,
                ImageGallery = entity.ImageGallery,
                Characteristics = entity.Characteristics,
                CategoryId = entity.CategoryId
            };
        }

        
                public static PriceList ToEntity(this PriceListCreateDto dto, int sellerId, User seller)
                {
                    return new PriceList
                    {
                        Name = dto.Name,
                        SellerId = sellerId,
                        Seller = seller,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        Description = dto.Description,
                        PriceListType = dto.PriceListType,
                        Currency = dto.Currency ?? "BYN"
                    };
                }
        public static void UpdateFromDto(this PriceList entity, PriceListUpdateDto dto)
        {
            entity.Name = dto.Name;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.Description = dto.Description;
            entity.PriceListType = dto.PriceListType;
            entity.Currency = dto.Currency;
        }

        public static PriceListResponseDto ToDto(this PriceList entity)
        {
            return new PriceListResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                SellerId = entity.SellerId,
                SellerName = entity.Seller?.Name ?? "Unknown",
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                Description = entity.Description,
                PriceListType = entity.PriceListType,
                Currency = entity.Currency,
                Products = entity.Products?.Select(p => new PriceListProductResponseDto
                {
                    ProductId = p.ProductId,
                    ProductName = p.Product?.Name ?? "Unknown",
                    ProductSKU = p.Product?.SKU ?? "Unknown",
                    SpecialPrice = p.SpecialPrice,
                    RegularPrice = p.Product?.Price ?? 0
                }).ToList() ?? new List<PriceListProductResponseDto>(),
                AllowedBuyers = entity.AllowedBuyers?.Select(b => b.ToShortInfoDto()).ToList() ?? new List<UserShortInfoDto>()
            };
        }

        public static CategoryResponseDto ToDto(this Category entity)
        {
            return new CategoryResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                ImageUrl = entity.ImageUrl
            };
        }

        public static Category ToEntity(this CategoryCreateDto dto)
        {
            return new Category
            {
                Name = dto.Name,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl
            };
        }

        public static void UpdateFromDto(this Category entity, CategoryUpdateDto dto)
        {
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.ImageUrl = dto.ImageUrl;
        }

        public static UserShortInfoDto ToShortInfoDto(this User entity)
        {
            return new UserShortInfoDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Email = entity.Email,
                UserType = entity.UserType,
                UserRole = entity.UserRole,
                INN = entity.INN == null ? "" : entity.INN
            };
        }

        public static void UpdateFromDto(this User entity, UserProfileUpdateDto dto)
        {
            entity.Name = dto.Name;
            entity.UNP = dto.INN;
            entity.LegalAddress = dto.LegalAddress;
            entity.ActualAddress = dto.ActualAddress;
            entity.Email = dto.Email;
            entity.Phone = dto.Phone;
            entity.BankName = dto.BankName;
            entity.BankAccount = dto.BankAccount;
            entity.BankBIK = dto.BankBIK;
        }

        public static UserProfileResponseDto ToProfileDto(this User entity)
        {
            return new UserProfileResponseDto
            {
                Id = entity.Id,
                Name = entity.Name,
                UserType = entity.UserType,
                UserRole = entity.UserRole,
                INN = entity.INN ?? "",
                OKPO = entity.OKPO,
                UNP = entity.UNP,
                LegalAddress = entity.LegalAddress,
                ActualAddress = entity.ActualAddress,
                Email = entity.Email,
                Phone = entity.Phone,
                BankName = entity.BankName,
                BankAccount = entity.BankAccount,
                BankBIK = entity.BankBIK,
                IsProfileComplete = IsProfileComplete(entity)
            };
        }

        private static bool IsProfileComplete(User user)
        {
            return !string.IsNullOrEmpty(user.Name) &&
                   user.Name != "Не указано" &&
                   !string.IsNullOrEmpty(user.UNP) &&
                   user.UNP != "000000000" &&
                   !string.IsNullOrEmpty(user.LegalAddress) &&
                   user.LegalAddress != "Не указано" &&
                   !string.IsNullOrEmpty(user.ActualAddress) &&
                   user.ActualAddress != "Не указано" &&
                   !string.IsNullOrEmpty(user.Phone) &&
                   user.Phone != "Не указано";
        }
    }
}