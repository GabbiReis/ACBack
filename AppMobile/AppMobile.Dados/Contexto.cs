using AppMobile.AppMobile.Dados.Configuration;
using AppMobile.Data.Configurations;
using AppMobile.Models;
using Microsoft.EntityFrameworkCore;

namespace AppMobile.AppMobile.Dados
{
    public class AppMobileContext : DbContext
    {
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Grupo> Grupos { get; set; }
        public DbSet<Convite> Convites { get; set; }
        public DbSet<Sorteio> Sorteios { get; set; }

        public AppMobileContext(DbContextOptions<AppMobileContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UsuariosConfiguration());
            modelBuilder.ApplyConfiguration(new GrupoConfiguration());
            modelBuilder.ApplyConfiguration(new ConviteConfiguration());
            modelBuilder.ApplyConfiguration(new SorteioConfiguration());
        }
    }
}
