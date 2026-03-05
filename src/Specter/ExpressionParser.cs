using System.Linq.Expressions;

namespace Specter;

public static class ExpressionParser
{
    public static (string MethodName, IMatcher[] Matchers) Parse(LambdaExpression lambda)
    {
        var body = lambda.Body;

        // Handle UnaryExpression wrapping (e.g., void method cast to object via Convert)
        if (body is UnaryExpression unary)
            body = unary.Operand;

        if (body is MethodCallExpression call)
        {
            var matchers = call.Arguments.Select(ExtractMatcher).ToArray();
            return (call.Method.Name, matchers);
        }

        if (body is MemberExpression member)
        {
            return ($"get_{member.Member.Name}", []);
        }

        throw new ArgumentException(
            "Expression body must be a method call or property access, e.g. x => x.Send(_, _) or x => x.Name");
    }

    private static IMatcher ExtractMatcher(Expression arg)
    {
        var value = Expression.Lambda(arg).Compile().DynamicInvoke();

        if (value is not null)
        {
            var type = value.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Matcher<>))
            {
                var innerProp = type.GetProperty(nameof(Matcher<object>.Inner))!;
                return (IMatcher)innerProp.GetValue(value)!;
            }
        }

        return new EqualityMatcher<object>(value!);
    }
}