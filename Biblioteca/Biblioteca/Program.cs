using System.Xml.Serialization;
using Biblioteca.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DbCtx>();
var app = builder.Build();


app.MapGet("/a", () => "Hello World!");



// - Cadastro de livros: Adição, edição e remoção de informações dos livros no acervo.

//CADASTRO Livro
//http://localhost:porta/Biblioteca/livro/cadastrar
app.MapPost("/Biblioteca/livro/cadastrar", ([FromBody] Livro livro, [FromServices] DbCtx ctx) => {

    ctx.Livros.Add(livro);
    ctx.SaveChanges();
    return Results.Created("",livro);

});

//UPDATE Livro
//http://localhost:porta/Biblioteca/livro/atualizar/{id}
app.MapPut("/Biblioteca/livro/atualizar/{id}", ([FromRoute]int id,[FromBody] Livro livroAtt, [FromServices] DbCtx ctx) => {
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


//LISTAR Livros
//GET: http://localhost:porta/Biblioteca/livro/listar
app.MapGet("/Biblioteca/livro/listar", ([FromServices] DbCtx ctx) =>
{  
    
    List<Livro>? livrosDisponiveis = [];
    if (ctx.Livros.ToList().Any())
    {
     foreach (var item in ctx.Livros.ToList())
        {  
            if(ctx.Emprestimos.Any(e => e.LivroId != item.Id)){
                livrosDisponiveis.Add(item);
            }
        }
        return Results.Ok(livrosDisponiveis);
    }
    return Results.NotFound("Tabela vazia!");
});

//BUSCAR Livro
//GET: http://localhost:5290/Biblioteca/livro/buscar/id_do_produto
app.MapGet("/Biblioteca/livro/buscar/{id}", ([FromRoute] int Id, [FromServices] DbCtx ctx) =>
{
    //Expressão lambda em c#
    Livro? livro = ctx.Livros.FirstOrDefault(x => x.Id == Id);
    if (livro is null)
    {
        return Results.NotFound("Livro não encontrado!");
    }
    return Results.Ok(livro);
});



//DELETE Livro
//http://localhost:5290/Biblioteca/livro/deletar/{id}
app.MapDelete("/Biblioteca/livro/deletar/{id}",
    ([FromRoute] int Id, 
    [FromServices] DbCtx ctx) =>
    {
        Livro? livro = ctx.Livros.Find(Id);
        if (livro is null){

            return Results.NotFound("Livro não Encontrado!");

        }
        ctx.Livros.Remove(livro);
        ctx.SaveChanges();
      return Results.Ok("Livro deletado Com sucesso");
});


// - Cadastro de usuários: Registro de novos usuários (leitores) da biblioteca.
// ########################################################################################################################

//CADASTRAR Usuario
// http://localhost:5290/Biblioteca/usuarios/cadastrar
app.MapPost("/Biblioteca/usuarios/cadastrar", ([FromBody] Usuario usuario, [FromServices] DbCtx ctx) => {

    ctx.Usuarios.Add(usuario);
    ctx.SaveChanges();
    return Results.Created("",usuario);



});


//LISTAR Usuarios
//GET: http://localhost:5290/Biblioteca/usuarios/listar
app.MapGet("/Biblioteca/usuarios/listar", ([FromServices] DbCtx ctx) =>
{
    if (ctx.Usuarios.ToList().Any())
    {
        return Results.Ok(ctx.Usuarios.ToList());
    }
    return Results.NotFound("Tabela vazia!");
});

//UPDATE Usuario
//http://localhost:porta/Biblioteca/usuarios/atualizar/{id}
app.MapPut("/Biblioteca/usuarios/atualizar/{id}", ([FromRoute]int id,[FromBody] Usuario usuarioAtt, [FromServices] DbCtx ctx) => {

    Usuario? usuarioParaAtt = ctx.Usuarios.Find(id);
    if (usuarioParaAtt == null){
        return Results.NotFound("Usuario nao encontrado");
    }

    usuarioParaAtt.Nome = usuarioAtt.Nome;
    usuarioParaAtt.Cpf = usuarioAtt.Cpf;
    usuarioParaAtt.Telefone = usuarioAtt.Telefone;
    usuarioParaAtt.Endereco = usuarioAtt.Endereco;

    ctx.Update(usuarioParaAtt);
    ctx.SaveChanges();
    return Results.Ok(usuarioParaAtt);
});

//BUSCAR usuarios
//GET: http://localhost:5290/Biblioteca/usuarios/buscar/id_do_usuario
app.MapGet("/Biblioteca/usuarios/buscar/{id}",([FromRoute] int Id, [FromServices] DbCtx ctx) =>
{
    //Expressão lambda em c#
    Usuario? usuario =
        ctx.Usuarios.FirstOrDefault(x => x.Id == Id);
    if (usuario is null)
    {
        return Results.NotFound("Usuario não encontrado!");
    }
    return Results.Ok(usuario);
});



// - Empréstimos de livros: Possibilidade de realizar empréstimos para os usuários cadastrados.
// ##########################################################################################################################

//EMPRESTIMO livro
//usar aqui
//POST: http://localhost:5290/Biblioteca/emprestimo/id do usuario que fará o emprestimo
app.MapPost("/Biblioteca/emprestimo/{id}", ([FromRoute] int Id, [FromBody] EmpModel empModel, [FromServices] DbCtx ctx) =>
{

    Usuario? usuario = ctx.Usuarios.FirstOrDefault(x => x.Id == Id);
    if (usuario is null)
    {
        return Results.NotFound("Usuario não encontrado!");
    }

    Livro? livro = ctx.Livros.FirstOrDefault(x => x.Id == empModel.Id);
    if (livro is null)
    {
        return Results.NotFound("Livro não encontrado!");
    }
    
// - Consultas de disponibilidade: Verificação da disponibilidade dos livros no acervo.
    if (ctx.Emprestimos.Any(e => e.LivroId == empModel.Id))
    {
        return Results.BadRequest("Ja foi emprestado");
    }


    Emprestimo? e = new Emprestimo(usuario.Id, livro.Id);

    ctx.Emprestimos.Add(e);
    ctx.SaveChanges();
    return Results.Ok(e);

});


//LISTA DE EMPRESTIMOS
//POST: http://localhost:5290/Biblioteca/emprestimo/listar
app.MapGet("/Biblioteca/emprestimo/listar", ([FromServices] DbCtx ctx) =>
{
    if (ctx.Emprestimos.ToList().Any())
    {
        foreach (var item in ctx.Emprestimos.ToList())
        {   
            Usuario? usuario = ctx.Usuarios.FirstOrDefault(x => x.Id == item.UsuarioId);
            item.UsuarioQueEmprestou = usuario;

            Livro? livro = ctx.Livros.FirstOrDefault(x => x.Id == item.LivroId);
            item.LivroEmprestado = livro;   
        }
    
        return Results.Ok(ctx.Emprestimos.ToList());
    }
    return Results.NotFound("Tabela vazia!");

});

// - Devoluções de livros: Processo de devolução de livros por parte dos usuários.

//usar aqui
//Devolucao livro
app.MapPost("/Biblioteca/devolucao/{id}", ([FromRoute] int Id, [FromBody] EmpModel empModel, [FromServices] DbCtx ctx) =>
{
    Usuario? usuario = ctx.Usuarios.FirstOrDefault(x => x.Id == Id);
    if (usuario is null)
    {
        return Results.NotFound("Usuario não encontrado!");
    }

    Livro? livro = ctx.Livros.FirstOrDefault(x => x.Id == empModel.Id);
    if (livro is null)
    {
        return Results.NotFound("Livro não encontrado!");
    }

    Emprestimo? emprestimo = ctx.Emprestimos.FirstOrDefault(e => e.UsuarioId == usuario.Id && e.LivroId == livro.Id);
    if (emprestimo is null)
    {
        return Results.NotFound("Emprestimo não encontrado!");
    } 
    
    ctx.Emprestimos.Remove(emprestimo);
    ctx.SaveChanges();
    return Results.Ok("Empréstimo removido com sucesso");
});


app.Run();
