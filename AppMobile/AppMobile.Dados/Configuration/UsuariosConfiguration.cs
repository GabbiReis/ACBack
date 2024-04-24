using AppMobile.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppMobile.AppMobile.Dados.Configuration
{
    public class UsuariosConfiguration : IEntityTypeConfiguration<Usuarios>
    {
        public void Configure(EntityTypeBuilder<Usuarios> builder)
        {
            builder.ToTable("Usuarios"); // Assuming no schema needed
            builder.HasKey(u => u.ID);

            builder.Property(u => u.ID)
                .HasColumnName("ID")
                .UseIdentityColumn()
                .HasColumnType("int");

            builder.Property(u => u.Nome)
                .HasColumnName("Nome")
                .HasColumnType("varchar(100)")
                .IsRequired(); // Make Nome required

            builder.Property(u => u.Email)
                .HasColumnName("Email")
                .HasColumnType("varchar(200)")
                .IsRequired(); // Make Email required

            builder.Property(u => u.Senha)
                .HasColumnName("Senha")
                .HasColumnType("varchar(60)")
                .IsRequired(); // Make Senha required

            builder.Property(u => u.Foto)
                .HasColumnName("Foto")
                .HasColumnType("varbinary(max)");
        }
    }
}
