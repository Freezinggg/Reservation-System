using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ReservationSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReservationSystem.Infrastructure.Configurations
{
    public class SeatCategoryConfiguration : IEntityTypeConfiguration<SeatCategory>
    {
        public void Configure(EntityTypeBuilder<SeatCategory> builder)
        {
            builder.ToTable("seat_category");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .IsRequired();

            builder.Property(x => x.Capacity)
                   .IsRequired();

            builder.Property(x => x.RemainingSeats)
                   .IsRequired();

            builder.HasOne<Event>()
                   .WithMany()
                   .HasForeignKey(x => x.EventId);

            builder.Property(x => x.CreatedAt)
                   .HasDefaultValueSql("now()");

            builder.HasIndex(x => new { x.EventId, x.Name })
                   .IsUnique();

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "ck_seat_category_capacity_positive",
                    "\"Capacity\" > 0");

                t.HasCheckConstraint(
                    "ck_seat_category_remaining_non_negative",
                    "\"RemainingSeats\" >= 0");

                t.HasCheckConstraint(
                    "ck_seat_category_remaining_within_capacity",
                    "\"RemainingSeats\" <= \"Capacity\"");
            });
        }
    }
}
