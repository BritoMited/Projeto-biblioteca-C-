namespace Biblioteca.models;
public class Livro{
public Livro(){

}

public Livro(string titulo, string autor, string editora, string categoria){

    Titulo = titulo;
    Autor = autor;
    Editora = editora;
    Categoria = categoria;

}

public string? Titulo{get ;set ;}
public string? Autor{get ;set ;}
public string? Editora{get ;set ;}
public string? Categoria{get ;set ;}
public int? Id{get ;set ;}


}