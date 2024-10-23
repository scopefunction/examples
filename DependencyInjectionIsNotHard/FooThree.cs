namespace DependencyInjectionIsNotHard;

public interface IFooAction
{
    void Bark();
}

public class FooThree : IFooAction
{
    private readonly FooTwo _fooTwo;
    private readonly FooOne _fooOne;

    public FooThree(FooTwo fooTwo, FooOne fooOne)
    {
        _fooTwo = fooTwo;
        _fooOne = fooOne;
    }

    public void Bark()
    {
        _fooOne.Bark();
        _fooTwo.Bark();
    }
}