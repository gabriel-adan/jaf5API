using Domain;
using Domain.Dtos;
using Domain.RepositoryInterfaces;
using Logic.BusinessRules;
using Logic.Contracts;
using SharpArch.Domain.PersistenceSupport;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Logic.Commons
{
    public class TurnsLogic : ITurnsLogic
    {
        private readonly IRepository<Hour> hoursRepository;
        private readonly IRepository<Team> teamsRepository;
        private readonly IRepository<Perfil> perfilsRepository;
        private readonly IFieldsRepository fieldsRepository;
        private readonly ITurnsRepository turnsRepository;
        private readonly IRepository<Player> playersRepository;

        public TurnsLogic(IRepository<Hour> hoursRepository, IRepository<Team> teamsRepository, IRepository<Perfil> perfilsRepository, IFieldsRepository fieldsRepository, ITurnsRepository turnsRepository, IRepository<Player> playersRepository)
        {
            this.hoursRepository = hoursRepository;
            this.teamsRepository = teamsRepository;
            this.perfilsRepository = perfilsRepository;
            this.fieldsRepository = fieldsRepository;
            this.turnsRepository = turnsRepository;
            this.playersRepository = playersRepository;
        }

        public TurnResultDto Request(DateTime date, int hourId, int teamId, int perfilId)
        {
            try
            {
                if (date < Helper.GetDateTimeZone())
                    throw new ArgumentException("No se puede solicitar un turno para una fecha pasada.");
                Hour hour = hoursRepository.Get(hourId);
                if (hour == null)
                    throw new ArgumentException("Horario inválido.");
                Team team = teamsRepository.Get(teamId);
                if (team == null)
                    throw new ArgumentException("Grupo no encontrado.");
                Player requestPlayer = team.Players.Where(p => p.Perfil.Id == perfilId).FirstOrDefault();
                if (requestPlayer == null)
                    throw new ArgumentException("No eres miembro de este grupo.");
                int amoutPlayers = (int)EPlayersOnTeam.MIN_NUMBER;
                if (team.Players.Count < amoutPlayers)
                    throw new ArgumentException(string.Format("El grupo {0} no tiene la cantidad de participantes suficientes para invitar, debe tener al menos {1} participantes.", team.Name, amoutPlayers));
                Field field = fieldsRepository.FindAvailable(date, hour);
                if (field == null)
                    throw new ArgumentException("No hay canchas disponibles.");

                turnsRepository.TransactionManager.BeginTransaction();

                Turn turn = new Turn();
                turn.Date = date;
                turn.FullName = team.Name;
                turn.State = EState.REQUESTED;
                turn.Field = field;
                turn.Hour = hour;
                turn.Team = team;
                turnsRepository.Save(turn);
                turnsRepository.SaveOrUpdate(turn);
                Camp camp = hour.Camp;
                requestPlayer.ConfirmDate = Helper.GetDateTimeZone();
                playersRepository.SaveOrUpdate(requestPlayer);

                IList<Player> players = team.Players.Where(p => p.Perfil.Id != perfilId).ToList();
                //Notificar a los jugadores
                foreach (Player player in players)
                {
                    if (player.ConfirmDate.HasValue)
                    {
                        player.ConfirmDate = null;
                        playersRepository.SaveOrUpdate(player);
                    }

                    Perfil perfil = player.Perfil;
                    string nofity = string.Format("Hola {0}! El grupo {1} quiere jugar el día {2} a Hs {3} en la cancha {4}, {5} {6}. ¿Te sumas?", perfil.Name, team.Name, turn.Date.ToString("dd/MM/yy"), hour.Time.ToString("HH:mm"), camp.Name, camp.Street, camp.Number);

                }

                turnsRepository.TransactionManager.CommitTransaction();

                TurnResultDto turnResult = new TurnResultDto();
                turnResult.Id = turn.Id;
                turnResult.DateTime = new DateTime(date.Year, date.Month, date.Day, hour.Time.Hours, hour.Time.Minutes, hour.Time.Seconds);
                turnResult.Field = turn.Field.Name;
                turnResult.Name = turn.FullName;
                return turnResult;
            }
            catch (ArgumentException ae)
            {
                turnsRepository.TransactionManager.RollbackTransaction();
                throw ae;
            }
            catch
            {
                turnsRepository.TransactionManager.RollbackTransaction();
                throw;
            }
        }

        public TurnResultDto Reserve(DateTime date, int hourId, string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentException("Debe ingresar el nombre de quien reserva.");
                if (date < Helper.GetDateTimeZone())
                    throw new ArgumentException("No se puede solicitar un turno para una fecha pasada.");
                Hour hour = hoursRepository.Get(hourId);
                if (hour == null)
                    throw new ArgumentException("Horario inválido.");
                Field field = fieldsRepository.FindAvailable(date, hour);
                if (field == null)
                    throw new ArgumentException("No hay canchas disponibles.");

                turnsRepository.TransactionManager.BeginTransaction();

                Turn turn = new Turn();
                turn.Date = date;
                turn.FullName = name;
                turn.State = EState.RESERVED;
                turn.Field = field;
                turn.Hour = hour;
                turnsRepository.Save(turn);

                IList<Turn> requestedTurns = turnsRepository.GetRequests(date, hour, field);
                foreach (Turn requestedTurn in requestedTurns)
                {
                    Field requestedField = fieldsRepository.FindAvailable(date, hour);
                    if (requestedField != null)
                    {
                        requestedTurn.Field = requestedField;
                        turnsRepository.SaveOrUpdate(requestedTurn);
                    }
                    else
                    {
                        //Notificar los turnos que quedan sin canchas
                    }
                }
                turnsRepository.TransactionManager.CommitTransaction();
                TurnResultDto turnResult = new TurnResultDto();
                turnResult.Id = turn.Id;
                turnResult.DateTime = new DateTime(turn.Date.Year, turn.Date.Month, turn.Date.Day, turn.Hour.Time.Hours, turn.Hour.Time.Minutes, turn.Hour.Time.Seconds);
                turnResult.Field = turn.Field.Name;
                turnResult.Name = turn.FullName;
                return turnResult;
            }
            catch (ArgumentException ae)
            {
                turnsRepository.TransactionManager.RollbackTransaction();
                throw ae;
            }
            catch
            {
                turnsRepository.TransactionManager.RollbackTransaction();
                throw;
            }
        }

        public TurnResultDto CreateTeamTurn(DateTime date, int hourId, string name, bool isPrivate, int perfilId)
        {
            try
            {
                if (string.IsNullOrEmpty(name))
                    throw new ArgumentException("Debe ingresar un nombre para el grupo.");
                Perfil perfil = perfilsRepository.Get(perfilId);
                if (perfil == null)
                    throw new ArgumentException("Perfil inválido.");
                Hour hour = hoursRepository.Get(hourId);
                if (hour == null)
                    throw new ArgumentException("Horario inválido.");
                Field field = fieldsRepository.FindAvailable(date, hour);
                if (field == null)
                    throw new ArgumentException("No hay canchas disponibles.");

                teamsRepository.TransactionManager.BeginTransaction();

                Team team = new Team();
                team.IsCompleted = false;
                team.IsPrivate = isPrivate;
                team.Name = name;
                team.Perfil = perfil;

                teamsRepository.Save(team);

                DateTime now = Helper.GetDateTimeZone();
                Player player = new Player();
                player.Perfil = perfil;
                player.Team = team;
                player.RequestDate = now;
                player.ConfirmDate = now;
                team.Players.Add(player);

                teamsRepository.SaveOrUpdate(team);

                Turn turn = new Turn();
                turn.Date = date;
                turn.FullName = team.Name;
                turn.State = EState.REQUESTED;
                turn.Field = field;
                turn.Hour = hour;
                turn.Team = team;

                turnsRepository.Save(turn);

                turnsRepository.SaveOrUpdate(turn);

                teamsRepository.TransactionManager.CommitTransaction();
                TurnResultDto turnResult = new TurnResultDto();
                turnResult.Id = turn.Id;
                turnResult.DateTime = new DateTime(turn.Date.Year, turn.Date.Month, turn.Date.Day, turn.Hour.Time.Hours, turn.Hour.Time.Minutes, turn.Hour.Time.Seconds);
                turnResult.Field = turn.Field.Name;
                turnResult.Name = turn.FullName;
                return turnResult;
            }
            catch (ArgumentException ae)
            {
                teamsRepository.TransactionManager.RollbackTransaction();
                throw ae;
            }
            catch
            {
                teamsRepository.TransactionManager.RollbackTransaction();
                throw;
            }
        }
    }
}
