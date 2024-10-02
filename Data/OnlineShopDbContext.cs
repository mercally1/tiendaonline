using Microsoft.EntityFrameworkCore;
using tienda.Models;

namespace tienda.Data
{
    public class OnlineShopDbContext: DbContext
    {
        public OnlineShopDbContext(DbContextOptions<OnlineShopDbContext> options) 
        : base(options)
        {

        }

        public DbSet<Categoria> Categorias { get; set; } = null!;

        public DbSet<Detalle_Pedido> DetallePedidos { get; set; } = null!;

        public DbSet<Direccion> Direcciones { get; set; } = null!;

        public DbSet<Pedido> Pedidos{ get; set; } = null!;

        public DbSet<Producto> Productos { get; set; } = null!;

        public DbSet<Rol> Roles { get; set; } = null!;

        public DbSet<Usuario> Usuarios{ get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
            .HasMany(u => u.Pedidos)
            .WithOne(p => p.Usuarios)
            .HasForeignKey(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Producto>()
            .HasMany(u => u.DetallePedidos)
            .WithOne(p => p.Producto)
            .HasForeignKey(p => p.ProductoId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pedido>()
            .HasMany(u => u.DetallePedidos)
            .WithOne(dp => dp.Pedidos)
            .HasForeignKey(p => p.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pedido>()
            .Ignore(p => p.Direccion);

            modelBuilder.Entity<Categoria>()
            .HasMany(u => u.Productos)
            .WithOne(p => p.Categoria)
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}