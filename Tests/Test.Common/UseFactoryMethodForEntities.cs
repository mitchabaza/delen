using Ploeh.AutoFixture;

namespace Delen.Test.Common
{
    public class UseFactoryMethodForEntities : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new UseFactoryMethodSpecimenBuilder());
        }
    }
}