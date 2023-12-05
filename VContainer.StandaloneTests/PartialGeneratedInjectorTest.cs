using System;
using NUnit.Framework;
using VContainer.Internal;

namespace VContainer.Tests
{
    public abstract partial class AbstractPartialClass
    {
        readonly int x;
    
        [Inject]
        public AbstractPartialClass(int x)
        {
            this.x = x;
        }
    }
    
    //Not generic or abstract
    public partial class BasicPartialClass
    {
        readonly int a1;
        readonly int a2;
    
        //Multiple private fields is supported
        [Inject] private int b1;
        [Inject] private int b2;
        
        //Multiple private properties is supported
        [Inject] private int C1 { get; set; }
        [Inject] private int C2 { get; set; }
        
        int d1;
        int d2;
    
        //Multiple private methods is supported
        [Inject]
        void SetD1(int d1)
        {
            this.d1 = d1;
        }
        
         [Inject]
         void SetD2(int d2)
         {
             this.d2 = d2;
         }
    
        //Multiple private constructors is supported as long as only one is [Inject]
        // [Inject]
        // private BasicPartialClass(int a1)
        // {
        //     this.a1 = a1;
        // }
    
        private BasicPartialClass(int a1, int a2)
        {
            this.a1 = a1;
            this.a2 = a2;
        }
        
        //Marked private constructor is used even if a public one with less parameters exists
        public BasicPartialClass()
        {
            a1 = 1;
            a2 = 2;
        }
    
        public partial class PublicPartialNestedClass
        {
            readonly int x;
    
            [Inject]
            public PublicPartialNestedClass(int x)
            {
                this.x = x;
            }
        }
    }
    
    public partial class GenericPartialClass<T>
    {
        readonly T x;
    
        [Inject]
        public GenericPartialClass(T x)
        {
            this.x = x;
        }
    }
    
    public partial class PrivatePartialConstructorClass
    {
        readonly int x;
    
        [Inject]
        PrivatePartialConstructorClass(int x)
        {
            this.x = x;
        }
    }
    
    public partial class PartialPrivateFieldClass
    {
        [Inject] int x;
    
        [Inject]
        public PartialPrivateFieldClass()
        {
            x = 0;
        }
    }
    
    public partial class PartialPrivatePropertyClass
    {
        [Inject] int X { get; set; }
    }
    
    public partial class PartialGenericMethodClass
    {
        string x;
    
        [Inject]
        public void SetX<T>(T value)
        {
            x = value.ToString();
        }
    }
    
    public class PartialGeneratedInjectorTest
    {
        partial class PartialPrivateNestedClass
        {
            readonly int x;

            [Inject]
            public PartialPrivateNestedClass(int x)
            {
                this.x = x;
            }
        }
        
        [Test]
        public void GeneratesInjectorTest()
        {
            AssertTypeHasGeneratedInjector(typeof(BasicPartialClass));
        }

        [Test]
        public void GenericNotSupportedTest()
        {
            AssertTypeIsMissingGeneratedInjector(typeof(GenericPartialClass<int>));
        }

        [Test]
        public void PublicNestedNotSupportedTest()
        {
            AssertTypeIsMissingGeneratedInjector(typeof(BasicPartialClass.PublicPartialNestedClass));
        }

        [Test]
        public void PrivateNestedNotSupportedTest()
        {
            AssertTypeIsMissingGeneratedInjector(typeof(PartialPrivateNestedClass));
        }

        [Test]
        public void AbstractNotSupportedTest()
        {
            AssertTypeIsMissingGeneratedInjector(typeof(AbstractPartialClass));
        }

        [Test]
        public void PrivateConstructorSupportedTest()
        {
            AssertTypeHasGeneratedInjector(typeof(PrivatePartialConstructorClass));
        }

        [Test]
        public void PrivateFieldSupported()
        {
            AssertTypeHasGeneratedInjector(typeof(PartialPrivateFieldClass));
        }

        [Test]
        public void PrivatePropertySupported()
        {
            AssertTypeHasGeneratedInjector(typeof(PartialPrivatePropertyClass));
        }

        [Test]
        public void GenericMethodNotSupported()
        {
            AssertTypeIsMissingGeneratedInjector(typeof(PartialGenericMethodClass));
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
}