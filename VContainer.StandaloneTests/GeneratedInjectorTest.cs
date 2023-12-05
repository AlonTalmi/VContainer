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

//Not generic, partial or abstract
public class BasicClass
{
    readonly int a1;
    readonly int a2;

    //Multiple public field is supported
    [Inject] public int b1;
    [Inject] public int b2;
    
    //Multiple public properties is supported (even if getter is private)
    [Inject] public int C1 { private get; set; }
    [Inject] public int C2 { private get; set; }
    
    int d1;
    int d2;

    //Multiple public methods is supported
    [Inject]
    public void SetD1(int d1)
    {
        this.d1 = d1;
    }
    
     [Inject]
     public void SetD2(int d2)
     {
         this.d2 = d2;
     }

    //Multiple constructors is supported as long as only one is [Inject]
    [Inject]
    public BasicClass(int a1)
    {
        this.a1 = a1;
    }

    public BasicClass(int a1, int a2)
    {
        this.a1 = a1;
        this.a2 = a2;
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

public class GenericMethodClass
{
    string x;

    [Inject]
    public void SetX<T>(T value)
    {
        x = value.ToString();
    }
}

/// <summary>
/// Tests for simple none-partial generators tests
/// </summary>
[TestFixture]
public class GeneratedInjectorTest
{
    class PrivateNestedClass
    {
        readonly int x;

        [Inject]
        public PrivateNestedClass(int x)
        {
            this.x = x;
        }
    }
    
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

    [Test]
    public void GenericMethodNotSupported()
    {
        AssertTypeIsMissingGeneratedInjector(typeof(GenericMethodClass));
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
}