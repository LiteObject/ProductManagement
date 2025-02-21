namespace ProductManagement.Core.Entities
{
    /// <summary>
    /// The base class BaseEntity allows us to centralize common properties and 
    /// behaviors that all domain entities will share. 
    /// </summary>
    public abstract class BaseEntity
    {
        public int Id { get; private init; }

        public DateTimeOffset CreatedOn { get; private set; } = DateTimeOffset.UtcNow;

        public DateTimeOffset LastModifiedOn { get; private set; } = DateTimeOffset.UtcNow;

        public string? CreatedBy { get; private set; } = "System";

        public string? LastModifiedBy { get; private set; } = "System";

        public void UpdateModifiedOn()
        {
            LastModifiedOn = DateTimeOffset.UtcNow;
        }
    }
}
