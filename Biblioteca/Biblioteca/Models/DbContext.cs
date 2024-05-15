using Microsoft.EntityFrameworkCore;

namespace Biblioteca.models;


public class DbCtx : DbContext
{
   
    public DbSet<Livro> Livros { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=BibliotecaDb.db");
    }
}