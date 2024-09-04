namespace Tasker.BLL.Interfaces
{
    public interface IDatedEntity
    {
        DateTimeOffset CreatedAt { get; set; }

        DateTimeOffset UpdatedAt { get; set; }
    }
}