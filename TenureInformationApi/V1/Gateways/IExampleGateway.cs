using System.Collections.Generic;
using TenureInformationApi.V1.Domain;

namespace TenureInformationApi.V1.Gateways
{
    public interface IExampleGateway
    {
        Entity GetEntityById(int id);

        List<Entity> GetAll();
    }
}
