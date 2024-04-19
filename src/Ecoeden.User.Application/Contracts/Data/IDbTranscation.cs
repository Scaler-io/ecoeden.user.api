namespace Ecoeden.User.Application.Contracts.Data
{
    public interface IDbTranscation
    {
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }
}
