using Domain;
using Domain.Dtos;
using Logic.BusinessRules;
using Logic.Contracts;
using SharpArch.Domain.PersistenceSupport;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Logic.Commons
{
    public class TeamsLogic : ITeamsLogic
    {
        private readonly IRepository<Team> teamsRepository;
        private readonly IRepository<Perfil> perfilsRepository;
        private readonly IRepository<Player> playersRepository;
        private readonly IRepository<Turn> turnsRepository;

        public TeamsLogic(IRepository<Team> teamsRepository, IRepository<Perfil> perfilsRepository, IRepository<Player> playersRepository, IRepository<Turn> turnsRepository)
        {
            this.teamsRepository = teamsRepository;
            this.perfilsRepository = perfilsRepository;
            this.playersRepository = playersRepository;
            this.turnsRepository = turnsRepository;
        }

        public bool RequestJoinToTeam(int turnId, int perfilId)
        {
            try
            {
                Turn turn = turnsRepository.Get(turnId);
                Helper.ThrowIfNull(turn, "Turno inválido.");
                Team team = turn.Team;
                Helper.ThrowIfNull(team, "No existe el grupo.");
                Helper.ThrowIf(team.Players.Count == (int)EPlayersOnTeam.MAX_NUMBER, "El grupo ya está completo.");
                
                Perfil perfil = perfilsRepository.Get(perfilId);
                Helper.ThrowIfNull(perfil, "Perfil inválido.");
                Player player = team.Players.Where(p => p.Perfil == perfil).FirstOrDefault();
                if (player != null)
                {
                    Helper.ThrowIf(player.ConfirmDate.HasValue, string.Format("Ya estas unido al grupo {0}.", team.Name));
                    Helper.ThrowIf(!player.ConfirmDate.HasValue, string.Format("Ya formas parte del grupo {0}.", team.Name));
                }
                DateTime now = Helper.GetDateTimeZone();
                player = new Player();
                player.Perfil = perfil;
                player.RequestDate = now;
                player.Team = team;
                playersRepository.Save(player);

                DateTime date = turn.Date;
                TimeSpan time = turn.Hour.Time;
                DateTime turnDateTime = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);
                Helper.ThrowIf(turnDateTime < now, "El turno ya venció pero quedas unido al grupo para la próxima convocatoria!!!");
                var difDate = turnDateTime.AddHours((int)ETurnsExpiration.HOURS);
                Helper.ThrowIf(difDate < now, "Las solicitudes de unión solo pueden hacerse con 1 Hora de anticipación, pero quedas unido al grupo para la próxima convocatoria!!!");
                
                Perfil owner = team.Perfil;
                //Enviar notificación de solicitud de unión al propietario del grupo

                return true;
            }
            catch
            {
                throw;
            }
        }

        public TurnResultDto ResponseJoinToTeam(int turnId, int playerId, bool isAccepted)
        {
            try
            {
                TurnResultDto turnResultDto = null;
                Turn turn = turnsRepository.Get(turnId);
                Helper.ThrowIfNull(turn, "Turno inválido.");
                Team team = turn.Team;
                Helper.ThrowIfNull(team, "Grupo inválido.");
                
                Player player = team.Players.Where(p => p.Id == playerId).FirstOrDefault();
                Helper.ThrowIfNull(player, "No existe la solicitud de unión.");

                Perfil perfil = team.Perfil;
                playersRepository.TransactionManager.BeginTransaction();
                DateTime date = turn.Date;
                TimeSpan time = turn.Hour.Time;
                DateTime turnDateTime = new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, time.Seconds);
                if (isAccepted)
                {
                    DateTime now = Helper.GetDateTimeZone();
                    player.ConfirmDate = now;
                    playersRepository.SaveOrUpdate(player);

                    IList<Player> players = team.Players.Where(p => p.ConfirmDate.HasValue).ToList();
                    int count = players.Count;
                    if (turnDateTime < now)
                    {
                        if(count >= (int)EPlayersOnTeam.MIN_NUMBER)
                            throw new ArgumentException("El turno ya se venció pero el grupo está completo para la próxima convocatoria!!!");
                        else
                            throw new ArgumentException("El turno ya se venció, solicita un nuevo turno");
                    }
                    else
                    {
                        var difDate = turnDateTime.AddHours((int)ETurnsExpiration.HOURS / 2);
                        Helper.ThrowIf(difDate < now, "Las solicitudes de unión solo se pueden confirmar con media hora de anticipación al horario del turno.");
                    }

                    if (count < (int)EPlayersOnTeam.MIN_NUMBER)
                    {
                        //Notificar a todos los jugadores del grupo aceptados que un nuevo integrante se sumó
                        IList<Player> playersToNotify = team.Players.Where(p => p.Perfil != perfil && p != player && p.ConfirmDate.HasValue).ToList();

                        //Notificar al jugador unido recientemente que su solicitud fué aceptada
                        Perfil perfilAccepted = player.Perfil;

                    }
                    else
                    {
                        if (count == (int)EPlayersOnTeam.MIN_NUMBER)
                        {
                            //Confirmar el turno y
                            turn.Success = true;
                            turnsRepository.SaveOrUpdate(turn);
                            //Notificar a todos los jugadores que el grupo se completó y que el turno fue asignado
                            IList<Player> playersToNotify = team.Players.Where(p => p.Perfil != perfil && p.ConfirmDate.HasValue).ToList();

                            turnResultDto = new TurnResultDto();
                            turnResultDto.Id = turnId;
                            turnResultDto.DateTime = turnDateTime;
                            turnResultDto.Field = turn.Hour.Camp.Name + " Cancha " + turn.Field.Name;
                            turnResultDto.Name = team.Name;
                        }
                    }
                }
                else
                {
                    playersRepository.Delete(player);
                }
                playersRepository.TransactionManager.CommitTransaction();
                
                return turnResultDto;
            }
            catch
            {
                playersRepository.TransactionManager.RollbackTransaction();
                throw;
            }
        }
    }
}
