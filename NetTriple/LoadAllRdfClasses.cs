using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NetTriple.Annotation;
using NetTriple.Emit;

namespace NetTriple
{
    /// <summary>
    /// Finds and loads all classes that are decorated with the RdfTypeAttribute
    /// attribute. "Loading" consists of generating converters for the class
    /// </summary>
    public static class LoadAllRdfClasses
    {
        private static readonly Dictionary<Type, IConverter> ConverterMap = new Dictionary<Type, IConverter>();
        private static readonly Dictionary<string, Type> SubjectMap = new Dictionary<string, Type>();
        private static readonly List<DeclaredRelation> DeclaredRelations = new List<DeclaredRelation>(); 

        /// <summary>
        /// Finds all classes that are decorated with the RdfTypeAttribute
        /// attribute. Does this by scanning all assemblies in scope. 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Type> Find()
        {
            return AppDomain.CurrentDomain.GetAssemblies().ToList()
                .SelectMany(a => a.GetTypes())
                .Where(t => Attribute.GetCustomAttribute(t, typeof(RdfTypeAttribute)) != null);
        }

        /// <summary>
        /// "Loading" consists of generating converters for the class
        /// </summary>
        public static void Load()
        {
            if (ConverterMap.Count > 0)
            {
                return;
            }

            var types = Find().ToList();
            var sourceCode = GetSourceCode(types);
            Compile(sourceCode, types);

            LoadSubjectTemplates(types);
            LoadRelationTemplates(types);
        }

        public static Type GetTypeForSubject(string subject)
        {
            var subj = subject.ToWashedRdfSubject();
            if (!ConverterMap.Any(p => p.Value.IsConverterForSubject(subj)))
            {
                return null;
            }

            return ConverterMap.Single(p => p.Value.IsConverterForSubject(subj)).Key;
        }

        public static string WhatHaveIGot()
        {
            return ConverterMap.Aggregate(
                new StringBuilder(),
                (sb, pair) =>
                {
                    sb.AppendFormat("{0}\r\n", pair.Key.Name);
                    return sb;
                }).ToString();
        }

        public static IEnumerable<DeclaredRelation> GetRelations()
        {
            return DeclaredRelations;
        }

        private static void LoadRelationTemplates(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                var typeAttrib = (RdfTypeAttribute)Attribute.GetCustomAttribute(type, typeof (RdfTypeAttribute));
                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var childAttrib = (RdfChildrenAttribute) Attribute.GetCustomAttribute(property, typeof (RdfChildrenAttribute));
                    if (childAttrib != null)
                    {
                        DeclaredRelations.Add(new DeclaredRelation(type, typeAttrib, childAttrib, property.PropertyType));
                    }
                }
            }
        }

        private static void LoadSubjectTemplates(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                SubjectMap.Add(GetSubjectTemplate(type), type);
            }
        }

        private static string GetSubjectTemplate(Type type)
        {
            var prop = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Single(p => Attribute.GetCustomAttribute(p, typeof(RdfSubjectAttribute)) != null);
            var attrib = (RdfSubjectAttribute)Attribute.GetCustomAttribute(prop, typeof(RdfSubjectAttribute));
            return attrib.Template.Replace("{0}", "");
        }

        /// <summary>
        /// Whether a converter for the given type is loaded.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasConverterFor(Type type)
        {
            return ConverterMap.ContainsKey(type);
        }

        public static IConverter GetConverter(Type type)
        {
            if (!ConverterMap.ContainsKey(type))
            {
                return null;
            }

            return ConverterMap[type];
        }

        public static IConverterLocator GetLocator()
        {
            return new ConverterLocator(ConverterMap, SubjectMap);
        }

        private static string GetSourceCode(IEnumerable<Type> types)
        {
            var classSources = types
                .Select(t => new SourceCodeGenerator(t).GetSourceCode())
                .Aggregate(
                    new StringBuilder(),
                    (sb, sourceCode) =>
                    {
                        sb.AppendLine(sourceCode);
                        sb.AppendLine();
                        return sb;
                    }).ToString();

            return TemplateResources.AssemblyTemplate.Replace("##CONVERTERS##", classSources);
        }

        private static void Compile(string sourceCode, IEnumerable<Type> types)
        {
            var tree = CSharpSyntaxTree.ParseText(sourceCode);
            var compilation = CSharpCompilation.Create("NetTripleEmitted.dll")
                .AddReferences(
                    new MetadataFileReference(typeof(object).Assembly.Location),
                    new MetadataFileReference(typeof(ParallelQuery).Assembly.Location),
                    new MetadataFileReference(typeof(LoadAllRdfClasses).Assembly.Location))
                .AddReferences(GetAssembliesOfTypes(types).Select(a => new MetadataFileReference(a.Location)))
                .AddSyntaxTrees(tree);

            Assembly assembly;
            using (var stream = new MemoryStream())
            {
                var compileResult = compilation.Emit(stream);
                if (!compileResult.Success)
                {
                    throw new InvalidOperationException(string.Format("The assembly could not be built, there are {0} diagnostic messages.", compileResult.Diagnostics.Count()));
                }

                assembly = Assembly.Load(stream.GetBuffer());
            }

            foreach (var type in types)
            {
                var name = string.Format("NetTriple.Conversion.{0}Converter", type.Name);
                var converterType = assembly.GetType(name);
                var converter = Activator.CreateInstance(converterType);
                ConverterMap[type] = (IConverter)converter;
            }
        }

        private static IEnumerable<Assembly> GetAssembliesOfTypes(IEnumerable<Type> types)
        {
            return types.Aggregate(
                new List<Assembly>(),
                (assemblies, t) =>
                {
                    if (!assemblies.Contains(t.Assembly))
                    {
                        assemblies.Add(t.Assembly);
                    }

                    return assemblies;
                });
        }
    }
}
