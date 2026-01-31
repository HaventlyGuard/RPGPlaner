using Microsoft.EntityFrameworkCore;
using TaskService.Models;

namespace TaskService.DataAcces;

public class TaskContext : DbContext
{
    public TaskContext(DbContextOptions<TaskContext> options) : base(options){}
    
    public  DbSet<Column> Columns { get; set; }
     public DbSet<Ticket> Tickets { get; set; }
     public DbSet<SubTicket> Subtickets { get; set;}
     public DbSet<Tag> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Column>(entity =>
        {
            entity.ToTable("column")
                .HasKey(c => c.ColumnId).HasName("column_id");
            entity.Property(c => c.Name).HasColumnName("name").HasMaxLength(65);
            entity.Property(c => c.isAutoComplete).HasColumnName("is_auto_complete").HasDefaultValue(false);
            entity.Property(c => c.Position).HasColumnName("position").HasDefaultValue(0);
            entity.Property(c => c.Color).HasColumnName("color").HasDefaultValue("B3B3B3").HasMaxLength(6);
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("ticket");
            entity.Property(c => c.ColumnId).HasColumnName("column_id");
            entity.Property(c => c.UserId).HasColumnName("user_id");
            entity.Property(c => c.Title).HasColumnName("title").HasMaxLength(65);
            entity.HasKey(c => c.TicketId).HasName("ticket_id");
            entity.Property(c => c.StartDate).HasColumnName("start_date");
            entity.Property(c => c.EndDate).HasColumnName("end_date");
            entity.Property(c => c.Description).HasColumnName("description").HasMaxLength(120);
            entity.Property(c => c.Priority).HasConversion<string>().HasColumnName("priority");
            entity.Property(c => c.Position).HasColumnName("position");
            entity.Property(c => c.Category).HasConversion<string>().HasColumnName("category");
            entity.Property(c =>  c.TaskType).HasConversion<string>().HasColumnName("task_type");
            entity.Property(c => c.isComplete).HasColumnName("is_complete").HasDefaultValue(false);
            entity.Property(c=>c.Color).HasColumnName("color").HasDefaultValue("B3B3B3").HasMaxLength(6);

            entity.HasOne<Column>()
                .WithMany(c => c.Tickets)
                .HasForeignKey(c => c.ColumnId)  
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany<Tag>().WithMany();
            
            entity.HasMany<SubTicket>().WithOne().HasForeignKey(t => t.TicketId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SubTicket>(entity =>
        {
            entity.ToTable("sub_ticket");
            entity.Property(c => c.TicketId).HasColumnName("ticket_id");
            entity.HasKey(c => c.SubTicketId).HasName("sub_ticket_id");
            entity.Property(c => c.TaskType).HasConversion<string>().HasColumnName("task_type");
            entity.Property(c => c.Title).HasColumnName("title").HasMaxLength(65);
            entity.Property(c => c.Description).HasColumnName("description").HasMaxLength(120);
            entity.Property(c => c.isComplete).HasColumnName("is_complete").HasDefaultValue(false);
            
        });

        modelBuilder.Entity<Tag>(entity =>
            {
                entity.ToTable("tags");
                entity.Property(c => c.Category).HasColumnName("category");
                entity.Property(c => c.TagColor).HasColumnName("tag_color");
                entity.HasKey(c => c.TagName).HasName("tag_name");
            }
        );
        
     






    }

}