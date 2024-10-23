namespace DependencyInjectionIsNotHard;

public class FooTwo : IFooAction
{
    private readonly FooOne _fooOne;

    public FooTwo(FooOne fooOne)
    {
        _fooOne = fooOne;
    }
    
    public void Bark()
    {
        _fooOne.Bark();
    }
}