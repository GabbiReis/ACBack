using AppMobile.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace AppMobile.AppMobile.Dados.Configuration
{
    public class SorteioConfiguration : IEntityTypeConfiguration<Sorteios>
    {
        public void Configure(EntityTypeBuilder<Sorteios> builder)
        {
            builder.ToTable("Sorteios", "AppMobile");
            builder.HasKey(s => s.ID);

            builder
                .Property(s => s.ID)
                .HasColumnName("ID")
                .UseIdentityColumn()
                .HasColumnType("int");

            builder
                .Property(s => s.ID_Grupo)
                .HasColumnName("ID_Grupo")
                .HasColumnType("int");

            builder
                .Property(s => s.ID_Usuario)
                .HasColumnName("ID_Usuario")
                .HasColumnType("int");

            builder
                .Property(s => s.ID_ParticipanteSorteado)
                .HasColumnName("ID_ParticipanteSorteado")
                .HasColumnType("int");

            builder
                .Property(s => s.DataSorteio)
                .HasColumnName("DataSorteio")
                .HasColumnType("datetime");
        }
    }
}
