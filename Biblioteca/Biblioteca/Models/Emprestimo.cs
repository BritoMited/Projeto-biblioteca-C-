namespace Biblioteca.models;
public class Emprestimo{

//Inicializando o Construtor Vazio
    public Emprestimo()
    {

    }

//Construtor com tudo que é utilizado para ter registro de um emprestimo

    public Emprestimo(int usuarioId, int livroId){
        UsuarioId = usuarioId;
        LivroId = livroId;
    }

    public int Id { get; set; }
    public int UsuarioId { get; set; }
    public int LivroId { get; set; }
    public Livro? LivroEmprestado { get; set; } 
    public Usuario? UsuarioQueEmprestou { get; set; }


}