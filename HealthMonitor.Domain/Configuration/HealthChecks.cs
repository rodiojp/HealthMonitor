using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using HealthMonitor.Domain.Configuration.Interfaces;

namespace HealthMonitor.Domain.Configuration
{
    public class HealthChecks : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement() => new HealthCheck();

        protected override object GetElementKey(ConfigurationElement element) => ((HealthCheck)element).Name;

        public HealthCheck this[int index] => BaseGet(index) as HealthCheck;

        public new HealthCheck this[string value]
        {
            get { return (HealthCheck)BaseGet(value); }
        }

        protected override string ElementName => "HealthCheck";

        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;

    }
}