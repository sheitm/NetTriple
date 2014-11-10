using System.Collections.Generic;
using System.Linq;
using NetTriple;
using NetTriple.Emit;

namespace Samples
{
    public class CatConverter : IConverter
    {
        public void Convert(object obj, IList<Triple> triples, IConverterLocator locator, string parentSubject = null,
            string parentReferencePredicate = null)
        {
            ConvertCore((NetTriple.Tests.TestDomain.Cat)obj, triples, locator, parentSubject, parentReferencePredicate);
        }

        public void Inflate(IEnumerable<string> triples, IInflationContext context, IConverterLocator locator)
        {
            var instance = new NetTriple.Tests.TestDomain.Cat();
            var tripleObjects = triples.Select(t => t.ToTriple()).ToList();
            context.Add(tripleObjects.First().Subject, instance);
            foreach (var triple in tripleObjects.Where(t => t.Predicate.Contains("http://psi.hafslund.no/elements/child")))
            {
                context.AddParentReference(triple.Object, instance);
            }
            InflateCore(instance, tripleObjects.ToList(), context);
        }

        public void Link(object obj, IInflationContext context)
        {
            LinkCore((NetTriple.Tests.TestDomain.Cat)obj, context);

        }

        private void LinkCore(NetTriple.Tests.TestDomain.Cat obj, IInflationContext context)
        {
            var s1 = obj.Name.ToString();
            var template = "<http://nettriple/cat/{0}>";
            var s = template.Replace("{0}", s1);
           
            var allEnemies = context.GetAll<NetTriple.Tests.TestDomain.AnimalBase>(s, "http://nettriple/animal/enemies");
            if (allEnemies != null && allEnemies.Count() > 0)
            {
                obj.Enemies = allEnemies;
            }
        }


        private void ConvertCore(NetTriple.Tests.TestDomain.Cat obj, IList<Triple> triples, IConverterLocator locator,
            string parentSubject, string parentReferencePredicate)
        {
            var s1 = obj.Name.ToString();
            var template = "<http://nettriple/cat/{0}>";
            var s = template.Replace("{0}", s1);
            if (BeingConverted.IsConverting(s)) return;
            BeingConverted.Converting(s);
            triples.Add(new Triple
            {
                Subject = s,
                Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Object = "<http://nettriple/cat>"
            });
            triples.Add(new Triple
            {
                Subject = s,
                Predicate = "<http://nettriple/animal/name>",
                Object = obj.Name.ToTripleObject()
            });
            if (obj.Enemies != null)
            {
                foreach (var child in obj.Enemies)
                {
                    locator.GetConverter(child.GetType()).Convert(child, triples, locator);
                    string co = null;
                    if (typeof(NetTriple.Tests.TestDomain.Cat) == child.GetType())
                    {
                        co = string.Format("<http://nettriple/cat/{0}>", child.Name.ToString());
                    }
                    if (typeof(NetTriple.Tests.TestDomain.Dog) == child.GetType())
                    {
                        co = string.Format("<http://nettriple/dog/{0}>", child.Name.ToString());
                    }

                    var p = "<http://nettriple/animal/enemies>";

                    triples.Add(new Triple { Subject = s, Predicate = p, Object = co });
                }
            }

            BeingConverted.Finished(s);
        }

        public void InflateCore(NetTriple.Tests.TestDomain.Cat obj, IList<Triple> triples, IInflationContext context)
        {
            Triple triple;
            triple = triples.First();
            obj.Name = triple.Subject.GetIdOfSubject();
            triple = triples.SingleOrDefault(t => t.Predicate == "<http://nettriple/animal/name>");
            if (triple != null)
            {
                obj.Name = triple.GetObject<System.String>();
            }

        }

        public bool IsConverterForSubject(string subject)
        {
            return subject == "http://nettriple/cat";
        }
    }

    public class DogConverter : IConverter
    {
        public void Convert(object obj, IList<Triple> triples, IConverterLocator locator, string parentSubject = null,
            string parentReferencePredicate = null)
        {
            ConvertCore((NetTriple.Tests.TestDomain.Dog)obj, triples, locator, parentSubject, parentReferencePredicate);
        }

        public void Inflate(IEnumerable<string> triples, IInflationContext context, IConverterLocator locator)
        {
            var instance = new NetTriple.Tests.TestDomain.Dog();
            var tripleObjects = triples.Select(t => t.ToTriple()).ToList();
            context.Add(tripleObjects.First().Subject, instance);
            foreach (var triple in tripleObjects.Where(t => t.Predicate.Contains("http://psi.hafslund.no/elements/child")))
            {
                context.AddParentReference(triple.Object, instance);
            }
            InflateCore(instance, tripleObjects.ToList(), context);
        }

        public void Link(object obj, IInflationContext context)
        {
            LinkCore((NetTriple.Tests.TestDomain.Dog)obj, context);

        }

        private void LinkCore(NetTriple.Tests.TestDomain.Dog obj, IInflationContext context)
        {
            var s1 = obj.Name.ToString();
            var template = "<http://nettriple/dog/{0}>";
            var s = template.Replace("{0}", s1);
            var allEnemies = context.GetAll<NetTriple.Tests.TestDomain.AnimalBase>(s, "http://nettriple/animal/enemies");
            if (allEnemies != null && allEnemies.Count() > 0)
            {
                obj.Enemies = allEnemies;
            }
        }


        private void ConvertCore(NetTriple.Tests.TestDomain.Dog obj, IList<Triple> triples, IConverterLocator locator,
            string parentSubject, string parentReferencePredicate)
        {
            var s1 = obj.Name.ToString();
            var template = "<http://nettriple/dog/{0}>";
            var s = template.Replace("{0}", s1);
            if (BeingConverted.IsConverting(s)) return;
            BeingConverted.Converting(s);
            triples.Add(new Triple
            {
                Subject = s,
                Predicate = "<http://www.w3.org/1999/02/22-rdf-syntax-ns#type>",
                Object = "<http://nettriple/dog>"
            });
            triples.Add(new Triple
            {
                Subject = s,
                Predicate = "<http://nettriple/animal/name>",
                Object = obj.Name.ToTripleObject()
            });
            if (obj.Enemies != null)
            {
                foreach (var child in obj.Enemies)
                {
                    locator.GetConverter(child.GetType()).Convert(child, triples, locator);
                    string co = null;
                    if (typeof(NetTriple.Tests.TestDomain.Cat) == child.GetType())
                    {
                        co = string.Format("<http://nettriple/cat/{0}>", child.Name.ToString());
                    }
                    if (typeof(NetTriple.Tests.TestDomain.Dog) == child.GetType())
                    {
                        co = string.Format("<http://nettriple/dog/{0}>", child.Name.ToString());
                    }

                    var p = "<http://nettriple/animal/enemies>";

                    triples.Add(new Triple { Subject = s, Predicate = p, Object = co });
                }
            }

            BeingConverted.Finished(s);
        }

        public void InflateCore(NetTriple.Tests.TestDomain.Dog obj, IList<Triple> triples, IInflationContext context)
        {
            Triple triple;
            triple = triples.First();
            obj.Name = triple.Subject.GetIdOfSubject();
            triple = triples.SingleOrDefault(t => t.Predicate == "<http://nettriple/animal/name>");
            if (triple != null)
            {
                obj.Name = triple.GetObject<System.String>();
            }

        }

        public bool IsConverterForSubject(string subject)
        {
            return subject == "http://nettriple/dog";
        }
    }

}
