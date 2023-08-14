using Microsoft.EntityFrameworkCore;

using todo.Models;

namespace todo.Data
{
    public class TodoDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<ProjectDetail> ProjectDetails { get; set; }
        public DbSet<TaskDetail> TasksDetals { get; set; }

        public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options)
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            /*modelBuilder.Entity<User>()
                .Ignore(p => p.Password)
                .Ignore(p => p.ProjectDetails)
                .Ignore(p => p.Projects)
                .Ignore(p => p.TasksDetails);

            modelBuilder.Entity<Project>()
                .Ignore(p => p.User);

            modelBuilder.Entity<Tasks>()
                .Ignore(p => p.TasksDetails);*/

            modelBuilder.Entity<ProjectDetail>().HasKey(p => new { p.UserId, p.ProjectId });
            modelBuilder.Entity<ProjectDetail>()
                .HasOne(p => p.Project)
                .WithMany(p => p.ProjectDetails)
                .HasForeignKey(p => p.ProjectId)
                .OnDelete(DeleteBehavior.Cascade); ;

            modelBuilder.Entity<ProjectDetail>()
                .HasOne(p => p.User)
                .WithMany(p => p.ProjectDetails)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TaskDetail>().HasKey(p => new { p.UserId, p.TasksId });
            modelBuilder.Entity<TaskDetail>()
                .HasOne(p => p.Tasks)
                .WithMany(p => p.TasksDetails)
                .HasForeignKey(p => p.TasksId)
                .OnDelete(DeleteBehavior.Cascade); ;

            modelBuilder.Entity<TaskDetail>()
                .HasOne(p => p.User)
                .WithMany(p => p.TasksDetails)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            /*modelBuilder.Entity<Project>().OwnsOne(e => e.User)
                .Metadata.PrincipalToDependent?.SetIsEagerLoaded(false);
            modelBuilder.Entity<Project>().OwnsOne(e => e.Tasks)
                .Metadata.PrincipalToDependent?.SetIsEagerLoaded(false);
            modelBuilder.Entity<Project>().OwnsOne(e => e.ProjectDetails)
                .Metadata.PrincipalToDependent?.SetIsEagerLoaded(false);

            modelBuilder.Entity<User>().OwnsOne(e => e.ProjectDetails)
                .Metadata.PrincipalToDependent?.SetIsEagerLoaded(false);
            modelBuilder.Entity<User>().OwnsOne(e => e.TasksDetails)
                .Metadata.PrincipalToDependent?.SetIsEagerLoaded(false);
            modelBuilder.Entity<User>().OwnsOne(e => e.Projects)
                .Metadata.PrincipalToDependent?.SetIsEagerLoaded(false);*/


            modelBuilder.Entity<User>().Navigation(e => e.Projects)
                .AutoInclude(false);
            modelBuilder.Entity<User>().Navigation(e => e.ProjectDetails)
                .AutoInclude(false);
            modelBuilder.Entity<User>().Navigation(e => e.TasksDetails)
                .AutoInclude(false);

            modelBuilder.Entity<ProjectDetail>().Navigation(e => e.Project)
                .AutoInclude(false);
            modelBuilder.Entity<ProjectDetail>().Navigation(e => e.User)
                .AutoInclude(false);

            modelBuilder.Entity<Tasks>().Navigation(e => e.TasksDetails)
                .AutoInclude(false);
            modelBuilder.Entity<Tasks>().Navigation(e => e.Project)
                .AutoInclude(false);

            modelBuilder.Entity<TaskDetail>().Navigation(e => e.Tasks)
                .AutoInclude(false);
            modelBuilder.Entity<TaskDetail>().Navigation(e => e.User)
                .AutoInclude(false);

        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var insertedEntries = this.ChangeTracker.Entries()
                                   .Where(x => x.State == EntityState.Added)
                                   .Select(x => x.Entity);

            foreach (var insertedEntry in insertedEntries)
            {
                var auditableEntity = insertedEntry as Auditable;
                //If the inserted object is an Auditable. 
                if (auditableEntity != null)
                {
                    auditableEntity.DateCreated = DateTimeOffset.UtcNow;
                }
            }

            var modifiedEntries = this.ChangeTracker.Entries()
                       .Where(x => x.State == EntityState.Modified)
                       .Select(x => x.Entity);

            foreach (var modifiedEntry in modifiedEntries)
            {
                //If the inserted object is an Auditable. 
                var auditableEntity = modifiedEntry as Auditable;
                if (auditableEntity != null)
                {
                    auditableEntity.DateUpdated = DateTimeOffset.UtcNow;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        /*public async Task<T> Delete(T entity)
{
   //If the type we are trying to delete is auditable, then we don't actually delete it but instead set it to be updated with a delete date. 
   if (typeof(Auditable).IsAssignableFrom(typeof(T)))
   {
       (entity as Auditable).DateDeleted = DateTimeOffset.UtcNow;
       _dbSet.Attach(entity);
       _context.Entry(entity).State = EntityState.Modified;
   }
   else
   {
       _dbSet.Remove(entity);
   }

   return entity;
}*/

    }
}
