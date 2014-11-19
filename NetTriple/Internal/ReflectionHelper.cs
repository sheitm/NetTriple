using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using NetTriple.Documentation;
using NetTriple.Internal;
using NodaTime;

namespace NetTriple.Annotation.Internal
{
    public static class ReflectionHelper
    {
        private static readonly Dictionary<Type, Func<string, object>> Deserialisers = new Dictionary
            <Type, Func<string, object>>
        {
            {typeof(string), WashStringObject},

            {typeof(int), v => int.Parse(v.Replace(',', '.'), CultureInfo.InvariantCulture)},
            {typeof(float), v => float.Parse(v.Replace(',', '.'), CultureInfo.InvariantCulture)},
            {typeof(double), v => double.Parse(v.Replace(',', '.'), CultureInfo.InvariantCulture)},
            {typeof(long), v => long.Parse(v.Replace(',', '.'), CultureInfo.InvariantCulture)},
            {typeof(decimal), v => decimal.Parse(v.Replace(',', '.'), CultureInfo.InvariantCulture)},
            {typeof(Instant), DeserializeInstant},
            {typeof(DateTime), DeserializeDateTime},
            {typeof(bool), DeserializeBoolean}
        };

        /// <summary>
        /// Finds the property being referenced within a lambda. For instance,
        /// in the lambda:
        ///    x => x.Name
        /// The property Name of the type of x would be returned.
        /// </summary>
        /// <param name="expressionToCheck"></param>
        /// <returns></returns>
        public static MemberInfo FindProperty(Expression expressionToCheck)
        {
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

        //public static string GetBody(Expression expression)
        //{
        //    if (ExpressionType.Lambda != expression.NodeType)
        //    {
        //        throw new ArgumentException("Only lambda expressions are supported.");
        //    }

        //    var s = expression.ToString();
        //    var index = s.IndexOf("=>");
        //    if (index < 0)
        //    {
        //        return s;
        //    }

        //    index = index + 2;
        //    return s.Substring(index, s.Length - index);

        //    //var lambda = (LambdaExpression) expression;
        //    //return lambda.ToString();
        //}

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

        /// <summary>
        /// Whether the given property is for an enumerable type
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool IsEnumerable(PropertyInfo property)
        {
            var type = property.PropertyType;
            return typeof (string) != type && typeof (IEnumerable).IsAssignableFrom(type);
        }

        /// <summary>
        /// Whether the property has a setter that is publically accessible
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static bool IsAssignable(PropertyInfo property)
        {
            return property.GetSetMethod() != null;
        }

        /// <summary>
        /// Deserializes the given string to the type of T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string input)
        {
            try
            {
                var s = RemoveTypeInfo(input);

                if (typeof(T).IsEnum)
                {
                    return (T)Enum.Parse(typeof(T), s);
                }

                if (!Deserialisers.ContainsKey(typeof (T)))
                {
                    throw new InvalidOperationException(string.Format("Deserialization of type {0} is not supported.", typeof(T).FullName));
                }

                return (T) Deserialisers[typeof (T)].Invoke(s);
            }
            catch (Exception e)
            {
                throw new FormatException(string.Format("Unable to deserialize string {0} to type {1}", input, typeof(T).FullName), e);
            }
        }

        private static string RemoveTypeInfo(string input)
        {
            return input.RemoveRdfTypeInfo();
        }

        public static string WashStringObject(string v)
        {
            if (string.IsNullOrWhiteSpace(v))
            {
                return v;
            }

            return v.RemoveLeadingAndTrailingQuotes().UnescapeLiteral();
        }

        private static object DeserializeDateTime(string v)
        {
            object o;
            if (v.Contains("^^"))
            {
                var d = v.Substring(1, v.IndexOf("^^") - 3);
                o = DateTime.Parse(d);
            }
            else
            {
                o = DateTime.Parse(v.RemoveLeadingAndTrailingQuotes());
            }

            return o;
        }

        private static object DeserializeBoolean(string v)
        {
            var vl = v.ToLower().RemoveRdfTypeInfo().RemoveLeadingAndTrailingQuotes();
            return vl == "true" || vl == "1";
        }

        private static object DeserializeInstant(string arg)
        {
            const string pattern = @"2[0-9]{3}-[0-1][0-9]-[0-3][0-9]T[0-9]{2}:[0-9]{2}:[0-9]{2}Z";

            var regex = new Regex(pattern);
            var match = regex.Match(arg);
            if (!match.Success)
            {
                throw new FormatException(string.Format("A valid UTC datetime is not contained in {0}", pattern));
            }

            return Instant.FromDateTimeUtc(DateTime.Parse(match.Value).ToUniversalTime());
        }
    }
}
