using ChatBot.Models;
using FluentValidation;

namespace ChatBot.Validators
{
    public class CreateSettingsValidator : AbstractValidator <Settings>
    {
        public CreateSettingsValidator()
        {
            RuleFor(x => x.Creativity)
                .InclusiveBetween(0, 1)
                .WithMessage("Kreatywność może tylko przyjmować wartości z zakresu od 0 do 1.");

            RuleFor(x => x.MaxTokens)
                .GreaterThan(0)
                .WithMessage("Liczba tokenów musi być dodatnia.");
        }
    }
}
