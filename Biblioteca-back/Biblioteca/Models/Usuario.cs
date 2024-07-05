namespace Biblioteca.models;
public class Usuario{
//Construtor Vazio
    public Usuario(){

    }
////Construtor com tudo que é utilizado para ter registro de um Usuario
    public Usuario(string nome, string cpf, string telefone,string endereco){

        Nome = nome;
        Cpf = cpf;
        Telefone = telefone;
        Endereco = endereco;

    }
//Getters and Setters da Classe
    public int Id{get;set;}
    public string? Nome {get; set;}
    public string? Cpf {get; set;}
    public string? Telefone {get;set;}
    public string? Endereco {get;set;}
  //  public List<Livro>? ListaLivros {get; set;} = [];


}