using NSpec;
using NSpec.Assertions.nUnit;

namespace SampleSpecs.Demo
{
    class todo_example : nspec
    {
        void soon()
        {
            it["everyone will have a drink"] = todo;
            xspecify = ()=> true.should_be_false();
        }
    }
}