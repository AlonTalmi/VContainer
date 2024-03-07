using System;

namespace VContainer
{
    public static class RegistrationBuilderExtensions
    {
        public static IRegistrationBuilder As<TInterface>(this IRegistrationBuilder builder)
            => builder.As(typeof(TInterface));

        public static IRegistrationBuilder As<TInterface1, TInterface2>(this IRegistrationBuilder builder)
            => builder.As(typeof(TInterface1), typeof(TInterface2));

        public static IRegistrationBuilder As<TInterface1, TInterface2, TInterface3>(this IRegistrationBuilder builder)
            => builder.As(typeof(TInterface1), typeof(TInterface2), typeof(TInterface3));

        public static IRegistrationBuilder As<TInterface1, TInterface2, TInterface3, TInterface4>(this IRegistrationBuilder builder)
            => builder.As(typeof(TInterface1), typeof(TInterface2), typeof(TInterface3), typeof(TInterface4));

        public static IRegistrationBuilder WithParameter<TParam>(this IRegistrationBuilder builder, TParam value)
            => builder.WithParameter(typeof(TParam), value);

        public static IRegistrationBuilder WithParameter<TParam>(this IRegistrationBuilder builder, Func<IObjectResolver, TParam> value)
            => builder.WithParameter(typeof(TParam), resolver => value(resolver));

        public static IRegistrationBuilder WithParameter<TParam>(this IRegistrationBuilder builder, Func<TParam> value)
            => builder.WithParameter(typeof(TParam), _ => value());
    }
}