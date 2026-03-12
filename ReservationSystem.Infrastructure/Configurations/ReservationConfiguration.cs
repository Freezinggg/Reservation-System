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
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder.ToTable("reservation");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Quantity)
                   .IsRequired();

            builder.Property(x => x.Status)
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .HasDefaultValueSql("now()");

            builder.Property(x => x.ExpiresAt)
                   .IsRequired();

            builder.HasOne<SeatCategory>()
                   .WithMany()
                   .HasForeignKey(x => x.SeatCategoryId);

            builder.ToTable(t =>
            {
                t.HasCheckConstraint(
                    "ck_reservation_quantity_positive",
                    "\"Quantity\" > 0");

                t.HasCheckConstraint(
                    "ck_reservation_expiration_valid",
                    "\"ExpiresAt\" > \"CreatedAt\"");

                t.HasCheckConstraint(
                    "ck_reservation_status_valid",
                    "\"Status\" IN (1,2,3,4)");
            });

            builder.HasIndex(x => x.ExpiresAt)
                   .HasDatabaseName("idx_reservation_expiration")
                   .HasFilter("\"Status\" = 3");
        }
    }
}
