using Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace Infraestructure.NHibernateMaps
{
    public class HourMap : IAutoMappingOverride<Hour>
    {
        public void Override(AutoMapping<Hour> mapping)
        {
            mapping.Map(x => x.Time).CustomType("TimeAsTimeSpan");
        }
    }
}
