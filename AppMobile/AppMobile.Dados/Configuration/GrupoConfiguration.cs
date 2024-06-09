using AppMobile.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppMobile.Data.Configurations
{
    public class GrupoConfiguration : IEntityTypeConfiguration<Grupos>
    {
        public void Configure(EntityTypeBuilder<Grupos> builder)
        {
            builder.ToTable("Grupos", "AppMobile");
            builder.HasKey(u => u.ID);

            builder.Property(u => u.ID)
                .HasColumnName("ID")
                .UseIdentityColumn()
                .HasColumnType("int");

            builder.Property(u => u.Nome)
                .HasColumnName("Nome")
                .HasColumnType("varchar(255)")
                .IsRequired();

            builder.Property(u => u.Icone)
                .HasColumnName("Icone")
                .HasColumnType("varbinary(max)");

            builder.Property(u => u.QuantidadeMaxParticipantes)
                .HasColumnName("QuantidadeMaxParticipantes")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(u => u.Valor)
                .HasColumnName("Valor")
                .HasColumnType("decimal(10, 2)")
                .IsRequired();

            builder.Property(u => u.DataRevelacao)
                .HasColumnName("DataRevelacao")
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(u => u.Descricao)
                .HasColumnName("Descricao")
                .HasColumnType("text")
                .IsRequired();
        }
    }
}
