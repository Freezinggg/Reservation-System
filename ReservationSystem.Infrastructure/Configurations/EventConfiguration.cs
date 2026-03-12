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
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Event");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .HasDefaultValueSql("now()");
        }
    }
}
