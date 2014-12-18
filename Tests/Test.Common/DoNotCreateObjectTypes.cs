using Ploeh.AutoFixture;

namespace Delen.Test.Common
{
    public class DoNotCreateObjectTypes: ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new IgnoreObjectTypesSpecimenBuilder());
        }
    }
}