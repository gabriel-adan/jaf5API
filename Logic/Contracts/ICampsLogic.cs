using Domain.Dtos;
using System.Collections.Generic;

namespace Logic.Contracts
{
    public interface ICampsLogic
    {
        CampDto Create(int customerId, string name, string street, string number, double longitude, double latitude, IList<string> fieldNames, int fieldCount);

        IList<FieldDto> GetFields(int campId);

        void EditFieldState(int fieldId, int campId, bool isEnabled);
    }
}
