using FluentValidation;
using FertilizerWarehouseAPI.DTOs;

namespace FertilizerWarehouseAPI.Validators
{
    public class CreateUserDtoValidator : AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Tên đăng nhập không được để trống")
                .Length(3, 50).WithMessage("Tên đăng nhập phải từ 3-50 ký tự")
                .Matches("^[a-zA-Z0-9_]+$").WithMessage("Tên đăng nhập chỉ được chứa chữ, số và dấu gạch dưới");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Định dạng email không hợp lệ")
                .MaximumLength(100).WithMessage("Email không được vượt quá 100 ký tự");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống")
                .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
                .WithMessage("Mật khẩu phải chứa ít nhất 1 chữ thường, 1 chữ hoa và 1 số");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Họ tên không được để trống")
                .Length(2, 100).WithMessage("Họ tên phải từ 2-100 ký tự");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Vai trò không được để trống")
                .Must(BeValidRole).WithMessage("Vai trò không hợp lệ");

            RuleFor(x => x.Phone)
                .Matches(@"^[\+]?[0-9\-\(\)\s]+$")
                .WithMessage("Số điện thoại không hợp lệ")
                .When(x => !string.IsNullOrEmpty(x.Phone));
        }

        private bool BeValidRole(string role)
        {
            var validRoles = new[] { "admin", "warehouse", "team_leader", "sales" };
            return validRoles.Contains(role);
        }
    }

    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Định dạng email không hợp lệ")
                .MaximumLength(100).WithMessage("Email không được vượt quá 100 ký tự")
                .When(x => !string.IsNullOrEmpty(x.Email));

            RuleFor(x => x.FullName)
                .Length(2, 100).WithMessage("Họ tên phải từ 2-100 ký tự")
                .When(x => !string.IsNullOrEmpty(x.FullName));

            RuleFor(x => x.Role)
                .Must(BeValidRole).WithMessage("Vai trò không hợp lệ")
                .When(x => !string.IsNullOrEmpty(x.Role));

            RuleFor(x => x.Phone)
                .Matches(@"^[\+]?[0-9\-\(\)\s]+$")
                .WithMessage("Số điện thoại không hợp lệ")
                .When(x => !string.IsNullOrEmpty(x.Phone));
        }

        private bool BeValidRole(string role)
        {
            var validRoles = new[] { "admin", "warehouse", "team_leader", "sales" };
            return validRoles.Contains(role);
        }
    }

    public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
    {
        public ChangePasswordDtoValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Mật khẩu hiện tại không được để trống");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Mật khẩu mới không được để trống")
                .MinimumLength(6).WithMessage("Mật khẩu mới phải có ít nhất 6 ký tự")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
                .WithMessage("Mật khẩu mới phải chứa ít nhất 1 chữ thường, 1 chữ hoa và 1 số");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Xác nhận mật khẩu không được để trống")
                .Equal(x => x.NewPassword).WithMessage("Mật khẩu xác nhận không khớp");
        }
    }

    public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email không được để trống")
                .EmailAddress().WithMessage("Định dạng email không hợp lệ");
        }
    }
}
