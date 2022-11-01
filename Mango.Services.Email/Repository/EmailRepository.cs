using Mango.Services.Email.DbContexts;
using Mango.Services.Email.Messages;
using Mango.Services.Email.Models;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.Email.Repository
{
    public class EmailRepository : IEmailRepository
    {
        private readonly DbContextOptions<ApplicationDbContext> _dbContext;
        public EmailRepository(DbContextOptions<ApplicationDbContext> dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task SendAndLogEmail(UpdatePaymentResultMessage message)
        {
            // implenent an email sender or call some other class library

            EmailLog email = new EmailLog
            {
                Email = message.Email,
                EmailSent = DateTime.Now,
                Log = $"Order - {message.OrderId} has been created successfully"
            };
            await using var _db = new ApplicationDbContext(_dbContext);
            _db.EmailLogs.Add(email);
            await _db.SaveChangesAsync();
        }
    }
}
