using Domain.Dtos;
using System;
using System.Collections.Generic;

namespace Logic.Contracts
{
    public interface ITurnsLogic
    {
        TurnResultDto Request(DateTime date, int hourId, int teamId, int perfilId);

        TurnResultDto Reserve(DateTime date, int hourId, string name);

        TurnResultDto CreateTeamTurn(DateTime date, int hourId, string name, bool isPrivate, int perfilId);

        IList<TurnTeamDto> ListByBufferZone(double longitude, double latitude, float radius);
    }
}
