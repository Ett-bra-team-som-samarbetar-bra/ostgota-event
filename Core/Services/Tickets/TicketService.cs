namespace Core.Services.Tickets;

public class TicketDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int EventId { get; set; }
    public decimal Price { get; set; }
    public string? Seat { get; set; } 
}

public class TicketService(Database DbContext) : ITicketService
{
    private readonly Database _db = DbContext;

    public async Task<List<TicketDTO>> GetAllTickets()
    {   
        var tickets = await _db.Tickets.ToListAsync();
        return GetTicketDTO(tickets);
    }

    public async Task<TicketDTO> AddTicket(TicketDTO dto)
    {   
        var user = await _db.Users.FirstOrDefaultAsync(e => e.Id == dto.UserId);
        var event1 = await _db.Events.FirstOrDefaultAsync(e => e.Id == dto.EventId);

        if (user == null || event1 == null)
        {
            return null!;
        }

        var newTicket = new Ticket() 
        {
            UserId = dto.UserId,
            User = user,
            EventId = dto.EventId,
            Event = event1,
            Price = dto.Price,
            Seat = dto.Seat
        };
        
        user.BuyTickets(event1, 1, newTicket);
        await _db.SaveChangesAsync();

        return dto;
    }



    public async Task<bool> RemoveTicket(int ticketId)
    {   
        /* Ticket ticket = await _db.Tickets.FirstOrDefaultAsync(
            t => t.Id == ticketId
        ); 

        if (ticket == null) 
            return false;



        //_db.Tickets.Remove(ticket);



        var user = ticket.User;
        Console.WriteLine(user);


        var event1 = ticket.Event;
        Console.WriteLine(event1);


        user.CancelTickets(event1);
        await _db.SaveChangesAsync();
 */
        return true;
    }

    public List<TicketDTO> GetTicketDTO(List<Ticket> tickets)
    {
        return tickets.Select(t => new TicketDTO
        {
            Id = t.Id,
            UserId = t.UserId,
            EventId = t.EventId,
            Price = t.Price,
            Seat = t.Seat
        }).ToList();
    }

    public TicketDTO GetTicketDTO(Ticket ticket)
    {
        return new TicketDTO
        {
            Id = ticket.Id,
            UserId = ticket.UserId,
            EventId = ticket.EventId,
            Price = ticket.Price,
            Seat = ticket.Seat
        };
    }
}
