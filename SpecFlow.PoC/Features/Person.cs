using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SpecFlow.PoC.Features;

public abstract class Person
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]       
    public Guid Id { get; set; }
    public string Name { get; set; }
    public DateTime BirthdayDate { get; set; }
}
    
/// <inheritdoc />
public class Employee : Person
{
}

public abstract class Entity
{
    /*[Key()]
    public Guid Id { get; set; }*/
}