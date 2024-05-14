namespace Biblioteca.models;
public class Usuario{

    public Usuario(){

    }

    public Usuario(string nome, string cpf, string telefone,string endereco){

        Nome = nome;
        Cpf = cpf;
        Telefone = telefone;
        Endereco = endereco;

    }

    public string? Nome {get; set;}
    public string? Cpf {get; set;}
    public string? Telefone {get;set;}
    public string? Endereco {get;set;}
    public int? Id{get;set;}

    public List<Livro>? ListaLivro {get; set;}


}