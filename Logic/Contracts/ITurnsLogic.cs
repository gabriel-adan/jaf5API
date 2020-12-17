using Domain.Dtos;
using System;

namespace Logic.Contracts
{
    public interface ITurnsLogic
    {
        TurnResultDto Request(DateTime date, int hourId, int teamId, int perfilId);

        TurnResultDto Reserve(DateTime date, int hourId, string name);

        TurnResultDto CreateTeamTurn(DateTime date, int hourId, string name, bool isPrivate, int perfilId);
    }
}
