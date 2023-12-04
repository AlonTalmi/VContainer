using System;
using NUnit.Framework;
using VContainer.Internal;

namespace VContainer.Tests;

public abstract class AbstractClass
{
    readonly int x;

    [Inject]
    public AbstractClass(int x)
    {
        this.x = x;
    }
}

//Not generic or abstract
public class BasicClass
{
    readonly int a;

    //Public field is supported
    [Inject] public int b;
    [Inject] public int C { get; set; }
    int d;

    [Inject]
    public void SetD(int d)
    {
        this.d = d;
    }

    [Inject]
    public BasicClass(int a)
    {
        this.a = a;
    }

    public BasicClass(int a, int b)
    {
        this.a = a;
        this.b = b;
    }

    public class PublicNestedClass
    {
        readonly int x;

        [Inject]
        public PublicNestedClass(int x)
        {
            this.x = x;
        }
    }
}

public class GenericClass<T>
{
    readonly T x;

    [Inject]
    public GenericClass(T x)
    {
        this.x = x;
    }
}

public class PrivateConstructorClass
{
    readonly int x;

    [Inject]
    PrivateConstructorClass(int x)
    {
        this.x = x;
    }
}

public class PrivateFieldClass
{
    [Inject] int x;

    [Inject]
    public PrivateFieldClass()
    {
        x = 0;
    }
}

public class PrivatePropertyClass
{
    [Inject] int X { get; set; }
}

[TestFixture]
public class GeneratedInjectorTest
{
    [Test]
    public void GeneratesInjectorTest()
    {
        AssertTypeHasGeneratedInjector(typeof(BasicClass));
    }
    
    [Test]
    public void GenericNotSupportedTest()
    {
        AssertTypeIsMissingGeneratedInjector(typeof(GenericClass<int>));
    }

    [Test]
    public void PublicNestedSupportedTest()
    {
        AssertTypeHasGeneratedInjector(typeof(BasicClass.PublicNestedClass));
    }
    
    [Test]
    public void PrivateNestedNotSupportedTest()
    {
        AssertTypeIsMissingGeneratedInjector(typeof(PrivateNestedClass));
    }
    
    [Test]
    public void AbstractNotSupportedTest()
    {
        AssertTypeIsMissingGeneratedInjector(typeof(AbstractClass));
    }

    [Test]
    public void PrivateConstructorNotSupportedTest()
    {
        AssertTypeIsMissingGeneratedInjector(typeof(PrivateConstructorClass));
    }

    [Test]
    public void PrivateFieldNotSupported()
    {
        AssertTypeIsMissingGeneratedInjector(typeof(PrivateFieldClass));
    }

    [Test]
    public void PrivatePropertyNotSupported()
    {
        AssertTypeIsMissingGeneratedInjector(typeof(PrivatePropertyClass));
    }

    static IInjector AssertTypeHasGeneratedInjector(Type type)
    {
        var injectorType = GeneratedInjectorType(type);
        Assert.That(injectorType, Is.Not.Null);
        
        var injector = InjectorCache.GetOrBuild(type); 
        Assert.That(injector, Is.TypeOf(injectorType));

        return injector;
    }

    static IInjector AssertTypeIsMissingGeneratedInjector(Type type)
    {
        var injectorType = GeneratedInjectorType(type);
        Assert.That(injectorType, Is.Null);
        
        var injector = InjectorCache.GetOrBuild(type);
        Assert.That(injector, Is.TypeOf<ReflectionInjector>());

        return injector;
    }
    
    static Type GeneratedInjectorType(Type type)
    {
        return type.Assembly.GetType(InjectorCache.GetGeneratedInjectorName(type), false);
    }
    
    class PrivateNestedClass
    {
        readonly int x;

        [Inject]
        public PrivateNestedClass(int x)
        {
            this.x = x;
        }
    }
}