using FluentValidation;
using ServiceDesk.Api.Dtos;

namespace ServiceDesk.Api.Validators;

public class CreateTicketValidator : AbstractValidator<CreateTicketDto>
{
    public CreateTicketValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("El título del ticket es obligatorio.")
            .MaximumLength(100).WithMessage("El título no puede exceder los 100 caracteres.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("La descripción es obligatoria.")
            .MinimumLength(10).WithMessage("Proporcione al menos 10 caracteres para describir el problema.");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("La prioridad seleccionada no es válida.");
    }
}