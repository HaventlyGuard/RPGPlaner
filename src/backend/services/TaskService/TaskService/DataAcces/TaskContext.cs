using Microsoft.EntityFrameworkCore;
using TaskService.Models;

namespace TaskService.DataAcces;

public class TaskContext : DbContext
{
    public TaskContext(DbContextOptions<TaskContext> options) : base(options){}
    
    DbSet<Column> Columns { get; set; }
    DbSet<Ticket> Tickets { get; set; }
    DbSet<SubTicket> Subtickets { get; set;}
    DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Column>(entity =>
        {
            entity.ToTable("column")
                .HasKey(c => c.ColumnId).HasName("column_id");
            entity.Property(c => c.Name).HasColumnName("name");
            entity.Property(c => c.isAutoComplete).HasColumnName("is_auto_complete").HasDefaultValue(false);
            entity.Property(c => c.Position).HasColumnName("position").HasDefaultValue(0);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("ticket");
            entity.Property(c => c.ColumnId).HasColumnName("column_id");
            entity.Property(c => c.UserId).HasColumnName("user_id");
            entity.Property(c => c.Title).HasColumnName("title");
            entity.HasKey(c => c.TicketId).HasName("ticket_id");
            entity.Property(c => c.StartDate).HasColumnName("start_date");
            entity.Property(c => c.EndDate).HasColumnName("end_date");
            entity.Property(c => c.Description).HasColumnName("description");
            entity.Property(c => c.Priority).HasConversion<string>().HasColumnName("priority");
            entity.Property(c => c.Position).HasColumnName("position");
            entity.Property(c => c.Category).HasConversion<string>().HasColumnName("category");
            entity.Property(c =>  c.TaskType).HasConversion<string>().HasColumnName("task_type");

            entity.HasOne<Ticket>()  
                .WithOne()         
                .HasForeignKey<Ticket>(t => t.ColumnId)
                .HasPrincipalKey<Column>(c => c.ColumnId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany<Tag>().WithMany().UsingEntity(t => t.ToTable("tags"));
            
            entity.HasMany<SubTicket>().WithOne().HasForeignKey(t => t.TicketId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SubTicket>(entity =>
        {
            entity.ToTable("sub_ticket");
            entity.Property(c => c.TicketId).HasColumnName("ticket_id");
            entity.HasKey(c => c.SubTicketId).HasName("sub_ticket_id");
            entity.Property(c => c.TaskType).HasConversion<string>().HasColumnName("task_type");
            entity.Property(c => c.Title).HasColumnName("title");
            entity.Property(c => c.Description).HasColumnName("description");
            
        });

        modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("tags");
                entity.Property(c => c.Category).HasColumnName("category");
                entity.Property(c => c.TagColor).HasColumnName("tag_color");
                entity.Property(c => c.TagName).HasColumnName("tag_name");
            }
        );
        
     






    }

}