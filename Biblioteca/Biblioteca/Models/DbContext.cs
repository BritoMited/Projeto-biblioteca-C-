using Microsoft.EntityFrameworkCore;

namespace Biblioteca.models;

//Setando as tabelas que vão ter no banco de dados
public class DbCtx : DbContext
{
   
    public DbSet<Livro> Livros { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Emprestimo> Emprestimos { get; set; }

//Aqui fala para usar o arquivo BibliotecaDB.db como fonte de dados
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=BibliotecaDb.db");
    }
}