using Biblioteca.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
builder.Services.AddDbContext<DbCtx>();



app.MapGet("/a", () => "Hello World!");



// - Cadastro de livros: Adição, edição e remoção de informações dos livros no acervo.
// - Consultas de disponibilidade: Verificação da disponibilidade dos livros no acervo.
// - Cadastro de usuários: Registro de novos usuários (leitores) da biblioteca.
// - Empréstimos de livros: Possibilidade de realizar empréstimos para os usuários cadastrados.
// - Devoluções de livros: Processo de devolução de livros por parte dos usuários.


app.MapPost("/Biblioteca/livro/cadastrar", ([FromBody] Livro livro, [FromServices] DbCtx ctx) => {

    ctx.Livros.Add(livro);
    ctx.SaveChanges();
    return Results.Created("foi",livro);

    // ctx.Update();
    // ctx.SaveChanges();

});
app.MapPut("/Biblioteca/livro/atualizar/{id}", ([FromBody]int id, Livro livroAtt, [FromServices] DbCtx ctx) => {

    Livro? livroParaAtt = ctx.Livros.Find(id);
    if (livroParaAtt == null){
        return Results.NotFound("Livro nao encontrado");
    }

    livroParaAtt.Titulo = livroAtt.Titulo;
    livroParaAtt.Categoria = livroAtt.Categoria;
    livroParaAtt.Autor = livroAtt.Autor;
    livroParaAtt.Editora = livroAtt.Editora;

    ctx.Update(livroParaAtt);
    ctx.SaveChanges();
    return Results.Ok(livroParaAtt);
});

app.Run();
