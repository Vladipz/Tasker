using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

using Tasker.BLL.Interfaces;

namespace Tasker.API.Helpers.Interceptors
{
    internal sealed class UptadeDatedEntityInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
        {
            if (eventData.Context is not null)
            {
                UpdateAuditableEntities(eventData.Context);
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private static void UpdateAuditableEntities(DbContext context)
        {
            var utcNow = DateTimeOffset.Now;
            var entities = context.ChangeTracker.Entries<IDatedEntity>().ToList();

            foreach (EntityEntry<IDatedEntity> entry in entities)
            {
                if (entry.State == EntityState.Added)
                {
                    SetCurrentPropertyValue(entry, nameof(IDatedEntity.CreatedAt), utcNow);
                }

                if (entry.State == EntityState.Modified)
                {
                    SetCurrentPropertyValue(entry, nameof(IDatedEntity.UpdatedAt), utcNow);
                }
            }

            static void SetCurrentPropertyValue(EntityEntry entry, string propertyName, DateTimeOffset utcNow) =>
                entry.Property(propertyName).CurrentValue = utcNow;
        }
    }
}