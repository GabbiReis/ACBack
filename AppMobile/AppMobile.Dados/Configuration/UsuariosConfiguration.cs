using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using AppMobile.Models;

public class UsuariosConfiguration : IEntityTypeConfiguration<Usuarios>
{
    public void Configure(EntityTypeBuilder<Usuarios> builder)
    {
        builder.ToTable("Usuarios", "AppMobile"); // Assuming "AppMobile" schema is correct
        builder.HasKey(u => u.ID);

        builder.Property(u => u.ID)
            .HasColumnName("ID")
            .UseIdentityColumn()
            .HasColumnType("int");

        builder.Property(u => u.Nome)
            .HasColumnName("Nome")
            .HasColumnType("varchar(100)")
            .IsRequired();

        builder.Property(u => u.Email)
            .HasColumnName("Email")
            .HasColumnType("varchar(200)")
            .IsRequired();

        builder.Property(u => u.Senha)
            .HasColumnName("Senha")
            .HasColumnType("varchar(60)")
            .IsRequired();

        builder.Property(u => u.Foto)
            .HasColumnName("Foto")
            .HasColumnType("varbinary(max)");
    }
}
