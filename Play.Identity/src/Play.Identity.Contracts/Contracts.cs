namespace Play.Identity.Contracts
{
    public class Contracts
    {
        public record DebitGil(Guid userId, decimal Gil, Guid CorrelationId);
        public record GilDebited(Guid CorrelationId);
    }
}
