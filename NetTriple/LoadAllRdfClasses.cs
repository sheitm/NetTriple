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
using NetTriple.Fluency;
using NodaTime;

namespace NetTriple
{
    /// <summary>
    /// Finds and loads all classes that are decorated with the RdfTypeAttribute
    /// attribute. "Loading" consists of generating converters for the class
    /// </summary>
    public static class LoadAllRdfClasses
    {
        private const string TypePredicate = "http://www.w3.org/1999/02/22-rdf-syntax-ns#type";

        private static readonly Dictionary<Type, IConverter> ConverterMap = new Dictionary<Type, IConverter>();
        private static readonly Dictionary<string, Type> SubjectMap = new Dictionary<string, Type>();
        private static readonly List<DeclaredRelation> DeclaredRelations = new List<DeclaredRelation>(); 
        private static readonly List<Assembly> LoadedAssemblies = new List<Assembly>();
        private static readonly List<IBuiltTransform> DeclaredTransforms = new List<IBuiltTransform>(); 

        /// <summary>
        /// Empties all inner structure. Should normally only be used
        /// in test scenarios
        /// </summary>
        public static void Clear()
        {
            ConverterMap.Clear();
            SubjectMap.Clear();
            DeclaredRelations.Clear();
            LoadedAssemblies.Clear();
        }

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

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                LoadFromAssembly(assembly);
            }
        }

        public static IEnumerable<IBuiltTransform> GetDeclaredTransforms()
        {
            return DeclaredTransforms;
        }

        public static void LoadTransforms(params IBuiltTransform[] transforms)
        {
            if (transforms == null)
            {
                return;
            }

            DeclaredTransforms.AddRange(transforms);

            var locator = new TransformLocator(transforms);
            foreach (var builtTransform in transforms)
            {
                builtTransform.SetLocator(locator);
            }

            LoadTypes(new List<Type>(), transforms);
        }

        public static void LoadFromAssemblyOf<T>()
        {
            LoadFromAssembly(typeof (T).Assembly);
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
            return SubjectMap.Aggregate(
                new StringBuilder(),
                (sb, pair) =>
                {
                    sb.AppendFormat("{0}    {1}\r\n", pair.Key, pair.Value.Name);
                    return sb;
                }).ToString();
        }

        public static IEnumerable<DeclaredRelation> GetRelations()
        {
            return DeclaredRelations;
        }

        public static void LoadFromAssembly(Assembly assembly)
        {
            if (LoadedAssemblies.Contains(assembly))
            {
                return;
            }

            LoadedAssemblies.Add(assembly);

            var types = assembly.GetTypes()
                .Where(t => Attribute.GetCustomAttribute(t, typeof(RdfTypeAttribute)) != null);

            LoadTypes(types, new List<IBuiltTransform>());
        }

        public static Type GetTypeForTriples(IEnumerable<Triple> triples)
        {
            var tpAngled = string.Format("<{0}>", TypePredicate);
            var typeTriple = triples.SingleOrDefault(triple => triple.Predicate == tpAngled || triple.Predicate == TypePredicate);
            if (typeTriple == null)
            {
                throw new ArgumentException("No triple with type predicate");
            }

            var transform = DeclaredTransforms.SingleOrDefault(t => string.Format("<{0}>", t.TypeString) == typeTriple.Object || t.TypeString == typeTriple.Object);
            if (transform == null)
            {
                throw new InvalidOperationException(string.Format("No declared transform for rdf type {0}", typeTriple.Object));
            }

            return transform.Type;
        }

        private static void LoadTypes(IEnumerable<Type> types, IEnumerable<IBuiltTransform> transforms)
        {
            var typs = types == null ? new List<Type>() : types.ToList();
            var trnsfrms = transforms == null ? new List<IBuiltTransform>() : transforms.ToList();
            var sourceCode = GetSourceCode(typs, trnsfrms);

            var typesForCompile = new List<Type>();
            typesForCompile.AddRange(typs);
            typesForCompile.AddRange(trnsfrms.Select(t => t.Type));

            Compile(sourceCode, typesForCompile);

            LoadSubjectTemplates(typs, trnsfrms);
            LoadRelationTemplates(typs, trnsfrms);
        }

        private static void LoadRelationTemplates(IEnumerable<Type> types, IEnumerable<IBuiltTransform> transforms)
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

        private static void LoadSubjectTemplates(IEnumerable<Type> types, IEnumerable<IBuiltTransform> transforms)
        {
            foreach (var type in types ?? new Type[0])
            {
                SubjectMap.Add(GetSubjectTemplate(type), type);
            }

            foreach (var transform in transforms ?? new IBuiltTransform[0])
            {
                var key = transform.SubjectSpecification.Template.Replace("{0}", "");
                if (SubjectMap.ContainsKey(key))
                {
                    throw new InvalidOperationException(string.Format("Transform with subject {0} already added.", key));
                }

                SubjectMap.Add(key, transform.Type);
            }
        }

        private static string GetSubjectTemplate(Type type)
        {
            var prop = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .SingleOrDefault(p => Attribute.GetCustomAttribute(p, typeof(RdfSubjectAttribute)) != null);

            if (prop != null)
            {
                var attrib = (RdfSubjectAttribute)Attribute.GetCustomAttribute(prop, typeof(RdfSubjectAttribute));
                return attrib.Template.Replace("{0}", "");
            }

            var cAttr = (RdfSubjectOnClassAttribute)Attribute.GetCustomAttribute(type, typeof (RdfSubjectOnClassAttribute));
            return cAttr.Template.Replace("{0}", "");
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

        private static string GetSourceCode(IEnumerable<Type> types, IEnumerable<IBuiltTransform> transforms)
        {
            var codeList = new List<string>();
            if (types != null)
            {
                codeList.AddRange(types.Select(t => new SourceCodeGenerator(t).GetSourceCode()));
            }

            if (transforms != null)
            {
                codeList.AddRange(transforms.Select(t => new SourceCodeGenerator(t).GetSourceCode()));
            }

            var classSources = codeList
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
                    new MetadataFileReference(typeof(LoadAllRdfClasses).Assembly.Location),
                    new MetadataFileReference(typeof(Instant).Assembly.Location))
                .AddReferences(GetAssembliesOfTypes(types).Select(a => new MetadataFileReference(a.Location)))
                .AddSyntaxTrees(tree);

            Assembly assembly;
            using (var stream = new MemoryStream())
            {
                var compileResult = compilation.Emit(stream);
                if (!compileResult.Success)
                {
                    var sb = new StringBuilder();
                    sb.AppendFormat("The assembly could not be built, there are {0} diagnostic messages.\r\n", compileResult.Diagnostics.Count());
                    var msg = compileResult.Diagnostics.Aggregate(
                        sb,
                        (builder, each) =>
                        {
                            builder.AppendFormat("{0}\r\n", each.GetMessage());
                            return builder;
                        });

                    msg.AppendLine("=========== SOURCE ==============");
                    msg.Append(sourceCode);

                    throw new InvalidOperationException(msg.ToString());
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
