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
                Helper.ThrowIfNull(camp, "Datos de cancha inválida.");
                Helper.ThrowIf(!camp.IsEnabled, "Cancha no disponible por el momento.");
                Helper.ThrowIf(hoursRepository.Exists(time, dayOfWeek, campId), "Ya existe el horario para el día de la semana indicado.");

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
                Helper.ThrowIfNull(hour, "El horario no existe.");
                Helper.ThrowIf(hour.IsEnabled == isEnabled, "El horario ya se encuentra " + (isEnabled ? "habilitado" : "deshabilitado"));
                hour.IsEnabled = isEnabled;
                hoursRepository.SaveOrUpdate(hour);
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
