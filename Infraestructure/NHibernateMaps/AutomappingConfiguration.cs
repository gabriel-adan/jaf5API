using System;
using System.Linq;
using FluentNHibernate;
using FluentNHibernate.Automapping;
using SharpArch.Domain.DomainModel;

namespace Infraestructure.NHibernateMaps
{
    public class AutomappingConfiguration : DefaultAutomappingConfiguration
    {
        public override bool ShouldMap(Type type)
        {
            return type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEntityWithTypedId<>));
        }

        public override bool ShouldMap(Member member)
        {
            return base.ShouldMap(member) && member.CanWrite;
        }

        public override bool AbstractClassIsLayerSupertype(Type type)
        {
            return type == typeof(EntityWithTypedId<>) || type == typeof(Entity);
        }
    }
}
