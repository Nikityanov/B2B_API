using B2B_API.Domain.Interfaces;

namespace B2B_API.Domain.Entities
{
    /// <summary>
    /// Базовая сущность с общими свойствами
    /// </summary>
    public abstract class BaseEntity : IAggregateRoot
    {
        public int Id { get; protected set; }
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public DateTime? ModifiedAt { get; protected set; }

        public void UpdateModifiedDate()
        {
            ModifiedAt = DateTime.UtcNow;
        }
    }
}
