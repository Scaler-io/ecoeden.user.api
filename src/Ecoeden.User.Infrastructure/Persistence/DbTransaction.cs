using Ecoeden.User.Application.Contracts.Data;

namespace Ecoeden.User.Infrastructure.Persistence
{
    public sealed class DbTransaction : IDbTranscation
    {
        private readonly UserDbContext _context;

        public DbTransaction(UserDbContext context)
        {
            _context = context;
        }

        public void BeginTransaction() => _context.Database.BeginTransaction();

        public void CommitTransaction() => _context.Database.CommitTransaction();

        public void RollbackTransaction() => _context.Database.RollbackTransaction();
    }
}
