using Domain;
using Domain.Dtos;

namespace Logic.Contracts
{
    public interface ICampsLogic
    {
        CampAccountDto Create(int customerId, string name, string street, string number, double longitude, double latitude);
    }
}
