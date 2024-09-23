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

        public DbSet<Categoria> categorias { get; set; } =null!;

        public DbSet<Detalle_Pedido> detalle_Pedidos { get; set; } =null!;

        public DbSet<Direccion> direccions { get; set; }=null!;

        public DbSet<Pedido> pedidos{ get; set; }=null!;

        public DbSet<Producto> productos { get; set; }=null!;

        public DbSet<Rol> roles { get; set; }=null!;

        public DbSet<Usuario> usuarios{ get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>()
            .HasMany(u => u.Pedidos)
            .WithOne(p => p.usuarios)
            .HasForeignKey(u => u.UsuarioId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Producto>()
            .HasMany(u => u.DetallePedidos)
            .WithOne(p => p.Producto)
            .HasForeignKey(u => u.ProductoId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pedido>()
            .HasMany(u => u.DetallePedidos)
            .WithOne(p => p.pedidos)
            .HasForeignKey(u => u.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pedido>()
            .Ignore(p => p.direccion);

            modelBuilder.Entity<Categoria>()
            .HasMany(u => u.Productos)
            .WithOne(p => p.Categoria)
            .HasForeignKey(u => u.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);

        }
    }
}