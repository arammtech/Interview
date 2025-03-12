namespace Muqabalati.Domain.Common
{
    public abstract class BaseEntity : IEntity<int>
    {
        public int Id { get; set; }
    }
}
