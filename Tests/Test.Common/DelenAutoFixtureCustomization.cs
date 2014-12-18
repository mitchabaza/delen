using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Ploeh.AutoFixture.Kernel;

namespace Delen.Test.Common
{
    public class DelenAutoFixtureCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize(new CompositeCustomization(
                new MultipleCustomization(),
                new AutoMoqCustomization(),
                new DoNotCreateObjectTypes(),
                new UseFactoryMethodForEntities()));

            fixture.Customizations.Add(new StableFiniteSequenceRelay());

            fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }
    }
}