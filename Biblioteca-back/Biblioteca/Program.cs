// Importa o namespace para serialização/desserialização XML
using System.Xml.Serialization;

// Importa os modelos de dados da aplicação
using Biblioteca.models;

// Importa classes relacionadas ao ASP.NET Core MVC
using Microsoft.AspNetCore.Mvc;

// Importa classes relacionadas ao Entity Framework Core
using Microsoft.EntityFrameworkCore;

// Cria um novo construtor de aplicativo web
var builder = WebApplication.CreateBuilder(args);

// Adiciona o contexto do banco de dados à aplicação
builder.Services.AddDbContext<DbCtx>();

builder.Services.AddCors(options =>
    options.AddPolicy("Acesso Total",
        configs => configs
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod())
);

// Constrói a aplicação
var app = builder.Build();


//Url para teste
app.MapGet("/a", () => "Hello World!");



// - Cadastro de livros: Adição, edição e remoção de informações dos livros no acervo.

//CADASTRO Livro
//http://localhost:porta/Biblioteca/livro/cadastrar
app.MapPost("/Biblioteca/livro/cadastrar", ([FromBody] Livro livro, [FromServices] DbCtx ctx) => {  
// Adiciona o novo livro à coleção de livros no contexto do banco de dados
    ctx.Livros.Add(livro);
// Salva as alterações no banco de dados
    ctx.SaveChanges();
// Retorna uma resposta Created (201) com o livro cadastrado 
    return Results.Created("",livro);

});

// Atualização de Livro
// URL: http://localhost:porta/Biblioteca/livro/atualizar/{id}

app.MapPut("/Biblioteca/livro/atualizar/{id}", ([FromRoute] int id, [FromBody] Livro livroAtt, [FromServices] DbCtx ctx) => {
    // Busca o livro existente no banco de dados pelo ID fornecido na rota
    Livro? livroParaAtt = ctx.Livros.Find(id);
    
    // Verifica se o livro foi encontrado
    if (livroParaAtt == null) {
        // Retorna uma resposta HTTP 404 (Not Found) se o livro não for encontrado
        return Results.NotFound("Livro não encontrado");
    }

    // Atualiza as propriedades do livro existente com os valores fornecidos
    livroParaAtt.Titulo = livroAtt.Titulo;
    livroParaAtt.Categoria = livroAtt.Categoria;
    livroParaAtt.Autor = livroAtt.Autor;
    livroParaAtt.Editora = livroAtt.Editora;

    // Marca o livro atualizado como modificado no contexto do banco de dados
    ctx.Update(livroParaAtt);
    
    // Salva as alterações no banco de dados
    ctx.SaveChanges();
    
    // Retorna uma resposta HTTP 200 (OK) com o livro atualizado
    return Results.Ok(livroParaAtt);
});



// Listar Livros Disponíveis
// URL: http://localhost:porta/Biblioteca/livro/listar

app.MapGet("/Biblioteca/livro/listar", ([FromServices] DbCtx ctx) =>
{
    // Inicializa uma lista para armazenar os livros disponíveis
    List<Livro> livrosDisponiveis = new List<Livro>();

    // Obtém a lista de todos os livros do banco de dados
    List<Livro> todosLivros = ctx.Livros.ToList();

    // Verifica se há livros na tabela
    if (todosLivros.Any())
    {
        // Itera sobre cada livro
        foreach (var livro in todosLivros)
        {
            // Verifica se o livro não está emprestado
            if (!ctx.Emprestimos.Any(e => e.LivroId == livro.Id))
            {
                // Adiciona o livro à lista de livros disponíveis
                livrosDisponiveis.Add(livro);
            }
        }

        // Retorna a lista de livros disponíveis com uma resposta HTTP 200 (OK)
        return Results.Ok(livrosDisponiveis);
    }

    // Retorna uma resposta HTTP 404 (Not Found) se não houver livros na tabela
    return Results.NotFound("Tabela vazia!");
});


// Buscar Livro
// URL: http://localhost:5290/Biblioteca/livro/buscar/{id}

app.MapGet("/Biblioteca/livro/buscar/{id}", ([FromRoute] int Id, [FromServices] DbCtx ctx) =>
{
    // Expressão lambda em C# para buscar o livro pelo ID
    Livro? livro = ctx.Livros.FirstOrDefault(x => x.Id == Id);

    // Verifica se o livro foi encontrado
    if (livro is null)
    {
        // Retorna uma resposta HTTP 404 (Not Found) se o livro não for encontrado
        return Results.NotFound("Livro não encontrado!");
    }

    // Retorna uma resposta HTTP 200 (OK) com o livro encontrado
    return Results.Ok(livro);
});



// Deletar Livro
// URL: http://localhost:5290/Biblioteca/livro/deletar/{id}

app.MapDelete("/Biblioteca/livro/deletar/{id}", ([FromRoute] int Id, [FromServices] DbCtx ctx) =>
{
    // Busca o livro pelo ID
    Livro? livro = ctx.Livros.Find(Id);

    // Se não encontrado, retorna 404
    if (livro is null)
    {
        return Results.NotFound("Livro não encontrado!");
    }

    // Remove o livro e salva as alterações
    ctx.Livros.Remove(livro);
    ctx.SaveChanges();

    // Retorna 200 com mensagem de sucesso
    return Results.Ok("Livro deletado com sucesso");
});


// ----------------------------------------------------------------------------------------------------

// - Cadastro de usuários: Registro de novos usuários (leitores) da biblioteca.

// Cadastrar Usuário
// URL: http://localhost:5290/Biblioteca/usuarios/cadastrar

app.MapPost("/Biblioteca/usuarios/cadastrar", ([FromBody] Usuario usuario, [FromServices] DbCtx ctx) => {
    // Adiciona o novo usuário à coleção de usuários no contexto do banco de dados
    ctx.Usuarios.Add(usuario);
    
    // Salva as alterações no banco de dados
    ctx.SaveChanges();
    
    // Retorna uma resposta HTTP 201 (Created) com o usuário cadastrado
    return Results.Created("", usuario);
});



// Listar Usuários
// URL: http://localhost:5290/Biblioteca/usuarios/listar

app.MapGet("/Biblioteca/usuarios/listar", ([FromServices] DbCtx ctx) =>
{
    // Verifica se há usuários na tabela de usuários
    if (ctx.Usuarios.ToList().Any())
    {
        // Retorna uma resposta HTTP 200 (OK) com a lista de usuários
        return Results.Ok(ctx.Usuarios.ToList());
    }
    
    // Se a tabela de usuários estiver vazia, retorna uma resposta HTTP 404 (Not Found)
    return Results.NotFound("Tabela vazia!");
});


// Atualizar Usuário
// URL: http://localhost:porta/Biblioteca/usuarios/atualizar/{id}

app.MapPut("/Biblioteca/usuarios/atualizar/{id}", ([FromRoute] int id, [FromBody] Usuario usuarioAtt, [FromServices] DbCtx ctx) => {
    // Busca o usuário existente no banco de dados pelo ID fornecido na rota
    Usuario? usuarioParaAtt = ctx.Usuarios.Find(id);
    
    // Verifica se o usuário foi encontrado
    if (usuarioParaAtt == null) {
        // Retorna uma resposta HTTP 404 (Not Found) se o usuário não for encontrado
        return Results.NotFound("Usuário não encontrado");
    }

    // Atualiza as propriedades do usuário existente com os valores fornecidos
    usuarioParaAtt.Nome = usuarioAtt.Nome;
    usuarioParaAtt.Cpf = usuarioAtt.Cpf;
    usuarioParaAtt.Telefone = usuarioAtt.Telefone;
    usuarioParaAtt.Endereco = usuarioAtt.Endereco;

    // Marca o usuário atualizado como modificado no contexto do banco de dados
    ctx.Update(usuarioParaAtt);
    
    // Salva as alterações no banco de dados
    ctx.SaveChanges();
    
    // Retorna uma resposta HTTP 200 (OK) com o usuário atualizado
    return Results.Ok(usuarioParaAtt);
});


// Buscar Usuário
// URL: http://localhost:5290/Biblioteca/usuarios/buscar/{id}

app.MapGet("/Biblioteca/usuarios/buscar/{id}", ([FromRoute] int Id, [FromServices] DbCtx ctx) =>
{
    // Utiliza uma expressão lambda em C# para buscar o usuário pelo ID
    Usuario? usuario = ctx.Usuarios.FirstOrDefault(x => x.Id == Id);

    // Verifica se o usuário foi encontrado
    if (usuario is null)
    {
        // Retorna uma resposta HTTP 404 (Not Found) se o usuário não for encontrado
        return Results.NotFound("Usuário não encontrado!");
    }

    // Retorna uma resposta HTTP 200 (OK) com o usuário encontrado
    return Results.Ok(usuario);
});

// Deletar Livro
// URL: http://localhost:5290/Biblioteca/livro/deletar/{id}

app.MapDelete("/Biblioteca/usuarios/deletar/{id}", ([FromRoute] int Id, [FromServices] DbCtx ctx) =>
{
    // Busca o usuario pelo ID
    Usuario? usuario = ctx.Usuarios.Find(Id);

    // Se não encontrado, retorna 404
    if (usuario is null)
    {
        return Results.NotFound("Usuario não encontrado!");
    }

    // Remove o usuario e salva as alterações
    ctx.Usuarios.Remove(usuario);
    ctx.SaveChanges();

    // Retorna 200 com mensagem de sucesso
    return Results.Ok("Usuario deletado com sucesso");
});



// ----------------------------------------------------------------------------------------------------------------

// - Empréstimos de livros: Possibilidade de realizar empréstimos para os usuários cadastrados.


// Emprestar Livro
// URL: http://localhost:5290/Biblioteca/emprestimo/{id}

app.MapPost("/Biblioteca/emprestimo/{id}", ([FromRoute] int Id, [FromBody] EmpModel empModel, [FromServices] DbCtx ctx) =>
{
    // Busca o usuário pelo ID fornecido na rota
    Usuario? usuario = ctx.Usuarios.FirstOrDefault(x => x.Id == Id);
    if (usuario is null)
    {
        // Retorna uma resposta HTTP 404 (Not Found) se o usuário não for encontrado
        return Results.NotFound("Usuário não encontrado!");
    }

    // Busca o livro pelo ID fornecido no corpo da requisição
    Livro? livro = ctx.Livros.FirstOrDefault(x => x.Id == empModel.Id);
    if (livro is null)
    {
        // Retorna uma resposta HTTP 404 (Not Found) se o livro não for encontrado
        return Results.NotFound("Livro não encontrado!");
    }
    
    // Verifica a disponibilidade do livro no acervo
    if (ctx.Emprestimos.Any(e => e.LivroId == empModel.Id))
    {
        // Retorna uma resposta HTTP 400 (Bad Request) se o livro já foi emprestado
        return Results.BadRequest("Livro já foi emprestado");
    }

    // Cria um novo objeto de empréstimo com o ID do usuário e do livro
    Emprestimo emprestimo = new Emprestimo(usuario.Id, livro.Id);

    // Adiciona o empréstimo ao contexto do banco de dados
    ctx.Emprestimos.Add(emprestimo);
    
    // Salva as alterações no banco de dados
    ctx.SaveChanges();
    
    // Retorna uma resposta HTTP 200 (OK) com o empréstimo realizado
    return Results.Ok(emprestimo);
});


// Listar Empréstimos
// URL: http://localhost:5290/Biblioteca/emprestimo/listar

app.MapGet("/Biblioteca/emprestimo/listar", ([FromServices] DbCtx ctx) =>
{
    // Verifica se há empréstimos na tabela de empréstimos
    if (ctx.Emprestimos.ToList().Any())
    {
        // Itera sobre cada empréstimo na lista
        foreach (var item in ctx.Emprestimos.ToList())
        {   
            // Busca o usuário que realizou o empréstimo pelo ID do usuário associado ao empréstimo
            Usuario? usuario = ctx.Usuarios.FirstOrDefault(x => x.Id == item.UsuarioId);
            // Associa o usuário encontrado ao empréstimo
            item.UsuarioQueEmprestou = usuario;

            // Busca o livro emprestado pelo ID do livro associado ao empréstimo
            Livro? livro = ctx.Livros.FirstOrDefault(x => x.Id == item.LivroId);
            // Associa o livro encontrado ao empréstimo
            item.LivroEmprestado = livro;   
        }
    
        // Retorna uma resposta HTTP 200 (OK) com a lista de empréstimos
        return Results.Ok(ctx.Emprestimos.ToList());
    }
    
    // Se a tabela de empréstimos estiver vazia, retorna uma resposta HTTP 404 (Not Found)
    return Results.NotFound("Tabela vazia!");
});


// Devolução de Livro
// URL: http://localhost:porta/Biblioteca/devolucao/{id}

app.MapPost("/Biblioteca/devolucao/{id}", ([FromRoute] int Id, [FromBody] EmpModel empModel, [FromServices] DbCtx ctx) =>
{
    // Busca o usuário pelo ID fornecido na rota
    Usuario? usuario = ctx.Usuarios.FirstOrDefault(x => x.Id == Id);
    if (usuario is null)
    {
        // Retorna uma resposta HTTP 404 (Not Found) se o usuário não for encontrado
        return Results.NotFound("Usuário não encontrado!");
    }

    // Busca o livro pelo ID fornecido no corpo da requisição
    Livro? livro = ctx.Livros.FirstOrDefault(x => x.Id == empModel.Id);
    if (livro is null)
    {
        // Retorna uma resposta HTTP 404 (Not Found) se o livro não for encontrado
        return Results.NotFound("Livro não encontrado!");
    }

    // Busca o empréstimo correspondente ao usuário e ao livro
    Emprestimo? emprestimo = ctx.Emprestimos.FirstOrDefault(e => e.UsuarioId == usuario.Id && e.LivroId == livro.Id);
    if (emprestimo is null)
    {
        // Retorna uma resposta HTTP 404 (Not Found) se o empréstimo não for encontrado
        return Results.NotFound("Empréstimo não encontrado!");
    } 
    
    // Remove o empréstimo do contexto do banco de dados
    ctx.Emprestimos.Remove(emprestimo);
    // Salva as alterações no banco de dados
    ctx.SaveChanges();
    
    // Retorna uma resposta HTTP 200 (OK) com uma mensagem de sucesso
    return Results.Ok("Empréstimo removido com sucesso");
});


app.UseCors("Acesso Total");
app.Run();
