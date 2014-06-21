using System;
using System.Linq;
using NSpec;
using NSpec.Domain;
using NUnit.Framework;
using NSpec.Assertions.nUnit;

namespace NSpecSpecs
{
    [TestFixture]
    [Category("ContextCollection")]
    public class describe_ContextCollection
    {
        private ContextCollection contexts;

        [SetUp]
        public void setup()
        {
            contexts = new ContextCollection();

            var context = new Context();

            context.AddExample(new Example());

            context.AddExample(new Example { Pending = true });

            context.AddExample(new Example { Exception = new Exception() });

            context.Tags.Add(Tags.Focus);

            contexts.Add(context);
        }

        [Test]
        public void should_aggregate_examples()
        {
            contexts.Examples().Count().should_be(3);
        }

        [Test]
        public void is_marked_with_focus()
        {
            contexts.AnyTaggedWithFocus().should_be_true();
        }

        [Test]
        public void should_aggregate_failures()
        {
            contexts.Failures().Count().should_be(1);
        }

        [Test]
        public void should_aggregate_pendings()
        {
            contexts.Pendings().Count().should_be(1);
        }

        [Test]
        public void should_trim_skipped_contexts()
        {
            contexts.Add(new Context());
            contexts[0].AddExample(new Example());
            contexts[0].Examples[0].HasRun = true;
            contexts.Count().should_be(2);
            contexts.TrimSkippedContexts();
            contexts.Count().should_be(1);
        }
    }
}