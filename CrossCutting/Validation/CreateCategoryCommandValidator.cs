using FluentValidation;
using B2B_API.Application.Commands;

namespace B2B_API.CrossCutting.Validation
{
    /// <summary>
    /// Валидатор для команды создания категории
    /// </summary>
    public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название категории обязательно для заполнения")
                .MaximumLength(100).WithMessage("Название категории не должно превышать 100 символов")
                .Matches(@"^[a-zA-Zа-яА-Я0-9\s\-_()]+$").WithMessage("Название категории содержит недопустимые символы");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Описание категории не должно превышать 500 символов")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.ImageUrl)
                .Must(BeValidUrl).WithMessage("Некорректный URL изображения")
                .When(x => !string.IsNullOrEmpty(x.ImageUrl));
        }

        private bool BeValidUrl(string? url)
        {
            if (string.IsNullOrEmpty(url))
                return true;

            return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
