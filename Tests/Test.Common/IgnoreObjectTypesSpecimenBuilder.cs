using Ploeh.AutoFixture.Kernel;

namespace Delen.Test.Common
{
    public class IgnoreObjectTypesSpecimenBuilder : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            if (request.ToString().Contains("System.Object"))

            {
                return null;
            }
            return new NoSpecimen(request);

        }
    }
}