using AutoMapper;
using Mango.Services.OrderAPI.DbContexts;
using Mango.Services.OrderAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.OrderAPI.Repository;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IMapper _mapper;

    public OrderRepository(ApplicationDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<bool> AddOrder(OrderHeader orderHeader)
    {
        await _dbContext.OrderHeaders.AddAsync(orderHeader);
        await _dbContext.SaveChangesAsync();
        return true;
    }
    public async Task UpdateOrderPaymentStatus(int orderHeaderId, bool paid)
    {
        var orderHeader = await _dbContext.OrderHeaders.FirstOrDefaultAsync(x => x.Id == orderHeaderId);
        if (orderHeader != null)
        {
            orderHeader.PaymentStatus = paid;
            _dbContext.OrderHeaders.Update(orderHeader);
            await _dbContext.SaveChangesAsync();
        }
    }
}
