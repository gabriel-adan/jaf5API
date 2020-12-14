using Domain.Dtos;
using System;
using System.Collections.Generic;

namespace Logic.Contracts
{
    public interface IHoursLogic
    {
        HourDto Create(TimeSpan time, int dayOfWeek, bool isEnabled, int campId);

        void EnableDisable(int hourId, bool isEnabled);

        IList<HourDto> List(int campId);
    }
}
