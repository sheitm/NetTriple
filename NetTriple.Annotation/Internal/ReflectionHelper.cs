using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NetTriple.Annotation.Internal
{
    public static class ReflectionHelper
    {
        public static MemberInfo FindProperty(Expression expressionToCheck)
        {
            //Expression expressionToCheck = lambdaExpression;

            bool done = false;

            while (!done)
            {
                switch (expressionToCheck.NodeType)
                {
                    case ExpressionType.Convert:
                        expressionToCheck = ((UnaryExpression)expressionToCheck).Operand;
                        break;
                    case ExpressionType.Lambda:
                        expressionToCheck = ((LambdaExpression)expressionToCheck).Body;
                        break;
                    case ExpressionType.MemberAccess:
                        var memberExpression = ((MemberExpression)expressionToCheck);

                        if (memberExpression.Expression.NodeType != ExpressionType.Parameter &&
                            memberExpression.Expression.NodeType != ExpressionType.Convert)
                        {
                            throw new ArgumentException(string.Format("Expression '{0}' must resolve to top-level member and not any child object's properties. Use a custom resolver on the child type or the AfterMap option instead.", expressionToCheck), "lambdaExpression");
                        }

                        MemberInfo member = memberExpression.Member;

                        return member;
                    default:
                        done = true;
                        break;
                }
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Gets the type of the property. For non-list and non-generic properties, this is
        /// the declared types. For generic and enumerable types this is the inner type.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static Type GetTypeOfProperty(this PropertyInfo property)
        {
            var pType = property.PropertyType;

            if (pType.IsArray)
            {
                return pType.GetElementType();
            }

            return pType.GenericTypeArguments == null || pType.GenericTypeArguments.Length == 0
                ? pType
                : pType.GenericTypeArguments[0];
        }
    }
}
