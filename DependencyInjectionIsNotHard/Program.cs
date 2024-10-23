
using System.Reflection;
using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic.CompilerServices;
using BindingFlags = System.Reflection.BindingFlags;

namespace DependencyInjectionIsNotHard;

public static class Program
{
    public static void Main()
    {
        var serviceCollection = new ServiceCollection();
        
        serviceCollection.AddTransient<FooOne>();
        serviceCollection.AddTransient<FooTwo>();
        serviceCollection.AddTransient<FooThree>();

        var sp = serviceCollection.BuildServiceProvider();

        var fooThree = sp.GetRequiredService<FooThree>();
        fooThree.Bark();
    }
}

public class BadServiceCollection
{
    private readonly Dictionary<Type, Type> _registeredTypes = new();

    public void AddTransient<T>()
    {
        _registeredTypes.TryAdd(typeof(T), typeof(T));
    }

    public void AddTransient<TInterface, TInstance>()
    {
        _registeredTypes.TryAdd(typeof(TInterface), typeof(TInstance));
    }
    
    public BadServiceProvider Build()
    {
        return new BadServiceProvider(_registeredTypes);
    }
}

public class BadServiceProvider
{
    private readonly Dictionary<Type, Type> _types;
    private readonly Dictionary<Type, List<Type>> _dependencyTree = new();

    public BadServiceProvider(Dictionary<Type, Type> types)
    {
        _types = types;
        CreateDependencyTree();
    }

    private void CreateDependencyTree()
    {
        foreach (var type in _types)
        {
            var constructorInfos = type.Value.GetConstructors();

            if (constructorInfos.Length > 1)
            {
                throw new ArgumentException("Too many tasty treats to choose from. There can only be one constructor, otherwise wtf must I do?");
            }

            var constructorInfo = constructorInfos.First();
            var constructorParameters = constructorInfo.GetParameters();
            var constructorTypes = new List<Type>();
            
            foreach (var parameter in constructorParameters)
            {
                if (_types.ContainsKey(parameter.ParameterType))
                {
                    constructorTypes.Add(parameter.ParameterType);
                }

                throw new Exception($"The tasty treat {parameter.ParameterType.FullName} was not registered in the service collection");
            }

            _dependencyTree.TryAdd(type.Value, constructorTypes);
        }
    }

    public T Resolve<T>()
    {
        if (!_dependencyTree.ContainsKey(typeof(T)))
        {
            throw new Exception("Stop! Get some help! The service you are requesting does not exist in the service collection.");
        }


        var typeInstances = new Dictionary<Type, object>();
        
        void ResolveForType(Type typeToResolve)
        {
            var typeDependencies = _dependencyTree[typeToResolve];

            if (typeDependencies.Count == 0)
            {
                typeInstances.Add(typeToResolve, Activator.CreateInstance(typeToResolve));
            }

            foreach (var type in typeDependencies)
            {
                ResolveForType(type);
            }
        }

        Activator.CreateInstance(typeof(T), BindingFlags.Instance, BindingFlags.CreateInstance,);
    }
}