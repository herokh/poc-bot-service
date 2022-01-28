using FluentValidation;
using Hero.Chatbot.Web.Models;

namespace Hero.Chatbot.Web.Validators
{
    public class FlightSearchCriteriaValidator : AbstractValidator<FlightSearchCriteriaViewModel>
    {
        public FlightSearchCriteriaValidator()
        {
            RuleFor(model => model.Origin)
                .NotEmpty();

            RuleFor(model => model.Destination)
                .NotEmpty();

            RuleFor(model => model.DepartureDate)
                .NotNull();

            RuleFor(model => model.ArrivalDate)
                .NotNull();
        }
    }
}
