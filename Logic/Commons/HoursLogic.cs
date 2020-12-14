using System;
using System.Collections.Generic;
using Domain;
using Domain.Dtos;
using Domain.RepositoryInterfaces;
using Logic.Contracts;
using SharpArch.Domain.PersistenceSupport;

namespace Logic.Commons
{
    public class HoursLogic : IHoursLogic
    {
        private readonly IHoursRepository hoursRepository;
        private readonly IRepository<Camp> campRepository;

        public HoursLogic(IHoursRepository hoursRepository, IRepository<Camp> campRepository)
        {
            this.hoursRepository = hoursRepository;
            this.campRepository = campRepository;
        }

        public HourDto Create(TimeSpan time, int dayOfWeek, bool isEnabled, int campId)
        {
            try
            {
                Camp camp = campRepository.Get(campId);
                if (camp == null)
                    throw new ArgumentException("Datos de cancha inválida.");
                if (!camp.IsEnabled)
                    throw new ArgumentException("Cancha no disponible por el momento.");
                if (!hoursRepository.Exists(time, dayOfWeek, campId))
                {
                    Hour hour = new Hour();
                    hour.Time = time;
                    hour.DayOfWeek = dayOfWeek;
                    hour.IsEnabled = isEnabled;
                    hour.Camp = camp;
                    hoursRepository.Save(hour);
                    return new HourDto()
                    {
                        Id = hour.Id,
                        Time = hour.Time,
                        DayOfWeek = hour.DayOfWeek,
                        IsEnabled = hour.IsEnabled,
                        CampId = campId
                    };
                }
                else
                    throw new ArgumentException("Ya existe el horario para el día de la semana indicado.");
            }
            catch
            {
                throw;
            }
        }

        public void EnableDisable(int hourId, bool isEnabled)
        {
            try
            {
                Hour hour = hoursRepository.Get(hourId);
                if (hour != null)
                {
                    if (hour.IsEnabled != isEnabled)
                    {
                        hour.IsEnabled = isEnabled;
                        hoursRepository.SaveOrUpdate(hour);
                    }
                    else
                        throw new ArgumentException("El horario ya se encuentra " + (isEnabled ? "habilitado" : "deshabilitado"));
                }
                else
                    throw new ArgumentException("El horario no existe.");
            }
            catch
            {
                throw;
            }
        }

        public IList<HourDto> List(int campId)
        {
            IList<HourDto> results = new List<HourDto>();
            try
            {
                var hours = hoursRepository.ToList(campId);
                foreach (Hour hour in hours)
                {
                    results.Add(new HourDto()
                    {
                        Id = hour.Id,
                        Time = hour.Time,
                        DayOfWeek = hour.DayOfWeek,
                        IsEnabled = hour.IsEnabled
                    });
                }
            }
            catch
            {
                throw;
            }
            return results;
        }
    }
}
