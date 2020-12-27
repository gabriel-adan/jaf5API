using Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace Infraestructure.NHibernateMaps
{
    public class CampMap : IAutoMappingOverride<Camp>
    {
        public void Override(AutoMapping<Camp> mapping)
        {
            mapping.Map(x => x.Longitude).Not.Insert().Not.Update();
            mapping.Map(x => x.Latitude).Not.Insert().Not.Update();
        }
    }
}
