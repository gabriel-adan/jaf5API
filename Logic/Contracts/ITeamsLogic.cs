using Domain.Dtos;

namespace Logic.Contracts
{
    public interface ITeamsLogic
    {
        bool RequestJoinToTeam(int turnId, int perfilId);

        TurnResultDto ResponseJoinToTeam(int turnId, int playerId, bool isAccepted);
    }
}
