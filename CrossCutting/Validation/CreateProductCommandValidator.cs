using FluentValidation;
using B2B_API.Application.Commands;

namespace B2B_API.CrossCutting.Validation
{
    /// <summary>
    /// Валидатор для команды создания продукта
    /// </summary>
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название продукта обязательно для заполнения")
                .MaximumLength(200).WithMessage("Название продукта не должно превышать 200 символов")
                .Matches(@"^[a-zA-Zа-яА-Я0-9\s\-_()]+$").WithMessage("Название продукта содержит недопустимые символы");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Описание продукта не должно превышать 1000 символов")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Количество на складе не может быть отрицательным");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Цена продукта должна быть больше нуля")
                .LessThanOrEqualTo(999999.99m).WithMessage("Цена продукта слишком большая");

            RuleFor(x => x.SKU)
                .NotEmpty().WithMessage("Артикул (SKU) обязателен для заполнения")
                .MaximumLength(50).WithMessage("Артикул не должен превышать 50 символов")
                .Matches(@"^[A-Z0-9\-_]+$").WithMessage("Артикул должен содержать только заглавные буквы, цифры, дефис и подчеркивание");

            RuleFor(x => x.Manufacturer)
                .MaximumLength(100).WithMessage("Название производителя не должно превышать 100 символов")
                .When(x => !string.IsNullOrEmpty(x.Manufacturer));

            RuleFor(x => x.Unit)
                .MaximumLength(20).WithMessage("Единица измерения не должна превышать 20 символов")
                .When(x => !string.IsNullOrEmpty(x.Unit));

            RuleFor(x => x.ImageUrl)
                .Must(BeValidUrl).WithMessage("Некорректный URL изображения")
                .When(x => !string.IsNullOrEmpty(x.ImageUrl));

            RuleFor(x => x.Characteristics)
                .MaximumLength(2000).WithMessage("Характеристики продукта не должны превышать 2000 символов")
                .When(x => !string.IsNullOrEmpty(x.Characteristics));

            RuleForEach(x => x.ImageGallery)
                .Must(BeValidUrl).WithMessage("Некорректный URL изображения в галерее")
                .When(x => x.ImageGallery != null);

            RuleFor(x => x.ImageGallery)
                .Must(gallery => gallery == null || gallery.Count <= 10)
                .WithMessage("Галерея изображений не может содержать более 10 изображений")
                .When(x => x.ImageGallery != null);
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
