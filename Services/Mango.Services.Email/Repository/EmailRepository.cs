using AutoMapper;
using Mango.Services.Email.DbContexts;
using Mango.Services.Email.Messages;
using Mango.Services.Email.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.Email.Repository;

public class EmailRepository : IEmailRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public EmailRepository(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task SendAndLogEmail(UpdatePaymentResultMessage message)
    {
        var emailLog = new EmailLog
        {
            Email = message.Email,
            Message = $"Order - {message.OrderId} has been created successfully.",
            Created = DateTime.Now,
        };

        _dbContext.Add(emailLog);
        await _dbContext.SaveChangesAsync();
    }
}
