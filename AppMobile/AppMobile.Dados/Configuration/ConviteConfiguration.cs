using AppMobile.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace AppMobile.AppMobile.Dados.Configuration
{
    public class ConviteConfiguration : IEntityTypeConfiguration<Convites>
    {
        public void Configure(EntityTypeBuilder<Convites> builder)
        {
            builder.ToTable("Convites", "AppMobile");
            builder.HasKey(c => c.ID);

            builder
                .Property(c => c.ID)
                .HasColumnName("ID")
                .UseIdentityColumn()
                .HasColumnType("int");

            builder
                .Property(c => c.ID_Grupo)
                .HasColumnName("ID_Grupo")
                .HasColumnType("int");

            builder
                .Property(c => c.ID_UsuarioConvidado)
                .HasColumnName("ID_UsuarioConvidado")
                .HasColumnType("int");

            builder
                .Property(c => c.Status)
                .HasColumnName("Status")
                .HasColumnType("varchar(50)");
        }
    }
}
