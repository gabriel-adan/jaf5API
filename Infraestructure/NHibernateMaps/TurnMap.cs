using Domain;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

namespace Infraestructure.NHibernateMaps
{
    public class TurnMap : IAutoMappingOverride<Turn>
    {
        public void Override(AutoMapping<Turn> mapping)
        {
            mapping.Map(x => x.State).Column("State_Id").CustomType<EState>();
        }
    }
}
