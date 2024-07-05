
namespace Biblioteca.models;
public class Livro{
//Inicializando o contrutor vazio
public Livro(){

}

////Construtor com tudo que é utilizado para ter registro de um Livro
public Livro(string titulo, string autor, string editora, string categoria){

    Titulo = titulo;
    Autor = autor;
    Editora = editora;
    Categoria = categoria;

}
//Getters e Setters da classe
public int Id{get ;set ;}
public string? Titulo{get ;set ;}
public string? Autor{get ;set ;}
public string? Editora{get ;set ;}
public string? Categoria{get ;set ;}
// public List<Usuario> ListaUsuarios { get; set; } = [];

 // Este operador permite converter implicitamente um Livro em List<object>.
    public static implicit operator List<object>(Livro v)
    {
        throw new NotImplementedException();
    }
}