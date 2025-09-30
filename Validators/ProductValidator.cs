using FluentValidation;
using FertilizerWarehouseAPI.DTOs;

namespace FertilizerWarehouseAPI.Validators
{
    public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Tên sản phẩm không được để trống")
                .Length(2, 200).WithMessage("Tên sản phẩm phải từ 2-200 ký tự");

            RuleFor(x => x.ProductCode)
                .NotEmpty().WithMessage("Mã sản phẩm không được để trống")
                .Length(2, 50).WithMessage("Mã sản phẩm phải từ 2-50 ký tự")
                .Matches("^[A-Z0-9-_]+$").WithMessage("Mã sản phẩm chỉ được chứa chữ hoa, số, dấu gạch ngang và gạch dưới");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Danh mục sản phẩm phải được chọn");

            RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Giá bán phải lớn hơn hoặc bằng 0");

            RuleFor(x => x.MinStockLevel)
                .GreaterThanOrEqualTo(0).WithMessage("Mức tồn kho tối thiểu phải lớn hơn hoặc bằng 0");

            RuleFor(x => x.Unit)
                .MaximumLength(20).WithMessage("Đơn vị tính không được vượt quá 20 ký tự")
                .When(x => !string.IsNullOrEmpty(x.Unit));

            RuleFor(x => x.Weight)
                .GreaterThan(0).WithMessage("Trọng lượng phải lớn hơn 0")
                .When(x => x.Weight.HasValue);

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }

    public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
    {
        public UpdateProductDtoValidator()
        {
            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("Tên sản phẩm không được để trống")
                .Length(2, 200).WithMessage("Tên sản phẩm phải từ 2-200 ký tự");

            RuleFor(x => x.ProductCode)
                .NotEmpty().WithMessage("Mã sản phẩm không được để trống")
                .Length(2, 50).WithMessage("Mã sản phẩm phải từ 2-50 ký tự");

            RuleFor(x => x.CategoryId)
                .GreaterThan(0).WithMessage("Danh mục sản phẩm phải được chọn");

            RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Giá bán phải lớn hơn hoặc bằng 0");

            RuleFor(x => x.MinStockLevel)
                .GreaterThanOrEqualTo(0).WithMessage("Mức tồn kho tối thiểu phải lớn hơn hoặc bằng 0");

            RuleFor(x => x.Unit)
                .MaximumLength(20).WithMessage("Đơn vị tính không được vượt quá 20 ký tự")
                .When(x => !string.IsNullOrEmpty(x.Unit));

            RuleFor(x => x.Weight)
                .GreaterThan(0).WithMessage("Trọng lượng phải lớn hơn 0")
                .When(x => x.Weight.HasValue);

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }

    public class CreateProductCategoryDtoValidator : AbstractValidator<CreateProductCategoryDto>
    {
        public CreateProductCategoryDtoValidator()
        {
            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("Tên danh mục không được để trống")
                .Length(2, 100).WithMessage("Tên danh mục phải từ 2-100 ký tự");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự")
                .When(x => !string.IsNullOrEmpty(x.Description));
        }
    }

    public class CreateProductBatchDtoValidator : AbstractValidator<CreateProductBatchDto>
    {
        public CreateProductBatchDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Sản phẩm phải được chọn");

            RuleFor(x => x.BatchNumber)
                .NotEmpty().WithMessage("Số lô không được để trống")
                .MaximumLength(100).WithMessage("Số lô không được vượt quá 100 ký tự");

            RuleFor(x => x.InitialQuantity)
                .GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0");

            RuleFor(x => x.ProductionDate)
                .LessThanOrEqualTo(DateTime.Today).WithMessage("Ngày sản xuất không được lớn hơn ngày hiện tại");

            RuleFor(x => x.ExpiryDate)
                .GreaterThan(x => x.ProductionDate).WithMessage("Ngày hết hạn phải lớn hơn ngày sản xuất");

            RuleFor(x => x.UnitCost)
                .GreaterThanOrEqualTo(0).WithMessage("Giá vốn phải lớn hơn hoặc bằng 0");

            RuleFor(x => x.SupplierId)
                .GreaterThan(0).WithMessage("Nhà cung cấp phải được chọn")
                .When(x => x.SupplierId.HasValue);
        }
    }
}
