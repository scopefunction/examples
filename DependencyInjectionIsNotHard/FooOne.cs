namespace DependencyInjectionIsNotHard;

public class FooOne : IFooAction
{
    public void Bark()
    {
        Console.WriteLine("Woof!");
    }
}