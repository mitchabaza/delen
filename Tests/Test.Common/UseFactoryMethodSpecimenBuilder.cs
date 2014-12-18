using Delen.Core.Entities;
using Ploeh.AutoFixture.Kernel;

namespace Delen.Test.Common
{
    public class UseFactoryMethodSpecimenBuilder : ISpecimenBuilder
    {

        private bool IsEntity(object request){

            if (request.GetType().IsAssignableFrom(typeof (Entity)))
            {
                return true;
            }
            return false;
        }
        public object Create(object request, ISpecimenContext context)
        {
            if (IsEntity(request))
            {
                return new MethodInvoker(new FactoryMethodQuery());
            }
            return new NoSpecimen(request);

        }
    }
}