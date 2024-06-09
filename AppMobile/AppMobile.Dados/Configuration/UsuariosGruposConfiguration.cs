using AppMobile.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AppMobile.AppMobile.Dados.Configuration
{
    public class UsuariosGruposConfiguration : IEntityTypeConfiguration<UsuariosGrupos>
    {
        public void Configure(EntityTypeBuilder<UsuariosGrupos> builder)
        {
            builder.ToTable("UsuariosGrupos", "AppMobile");

            builder.HasKey(ug => new { ug.UsuarioId, ug.GrupoId });

            builder.HasOne(ug => ug.Usuario)
                .WithMany(u => u.UsuariosGrupos)
                .HasForeignKey(ug => ug.UsuarioId);

            builder.HasOne(ug => ug.Grupo)
                .WithMany(g => g.UsuariosGrupos)
                .HasForeignKey(ug => ug.GrupoId);

            // Aqui você pode configurar outras propriedades, se necessário
            builder.Property(ug => ug.DataEntrada)
                .IsRequired();
        }
    }
}
