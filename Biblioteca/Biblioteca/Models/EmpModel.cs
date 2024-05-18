namespace Biblioteca.models;
//Classe para pegar o id do livro que vai ser emprestado

public class EmpModel{

    public EmpModel(){
        
    }
    public EmpModel(int id){
        Id = id;
    }

    public int? Id { get; set; }


}