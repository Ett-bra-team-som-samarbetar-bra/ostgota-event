﻿
namespace Api.Controllers.EventValidation;

public class EventValidator : AbstractValidator<Event>
{
    public EventValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
        RuleFor(x => x.City).NotEmpty();
        RuleFor(x => x.Adress).NotEmpty();
        RuleFor(x => x.StartTime).NotEmpty();
        RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime);
        RuleFor(x => x.TicketsMax).GreaterThan(0);
        RuleFor(x => x.TicketsSold).LessThanOrEqualTo(x => x.TicketsMax);
        RuleFor(x => x.RemainingTickets).GreaterThanOrEqualTo(0);
        RuleFor(x => x.IsSoldOut).Equal(x => x.RemainingTickets == 0);
        RuleFor(x => x.AccessType).IsInEnum();
    }

}