using Microsoft.EntityFrameworkCore;

//namespace API.Models;


public class AppDataContext : DbContext
{
   
    //public DbSet<Produto> Produtos { get; set; }

    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=app.db");
    }
}