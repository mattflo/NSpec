using NSpec;
using NSpecSpecs.WhenRunningSpecs;
using NUnit.Framework;
using System.Threading.Tasks;

namespace NSpecSpecs.describe_RunningSpecs.describe_before_and_after
{
    [TestFixture]
    [Category("Async")]
    public class async_abstract_class : when_running_specs
    {
        abstract class Abstract : sequence_spec
        {
            async Task before_all()
            {
                await Task.Run(() => sequence = "A");
            }

            async Task before_each()
            {
                await Task.Run(() => sequence += "C");
            }

            void a_context()
            {
                asyncBeforeAll = async () => await Task.Run(() => sequence += "B");

                asyncBefore = async () => await Task.Run(() => sequence += "D");
                specify = () => 1.Is(1);
                asyncAfter = async () => await Task.Run(() => sequence += "E");

                asyncAfterAll = async () => await Task.Run(() => sequence += "G");
            }

            async Task after_each()
            {
                await Task.Run(() => sequence += "F");
            }

            void after_all() // TODO-ASYNC
            {
                sequence += "H";
            }
        }

        class Concrete : Abstract {}

        [Test]
        public void all_async_features_are_supported_from_abstract_classes_when_run_under_the_context_of_a_derived_concrete()
        {
            Run(typeof(Concrete));
            Concrete.sequence.Is("ABCDEFGH");
        }
    }
}
