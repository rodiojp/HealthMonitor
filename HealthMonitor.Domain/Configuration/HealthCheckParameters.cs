using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using HealthMonitor.Domain.Configuration.Interfaces;

namespace HealthMonitor.Domain.Configuration
{
    public class HealthCheckParameters : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement() => new HealthCheckParameter();

        protected override object GetElementKey(ConfigurationElement element) => ((HealthCheckParameter)element).Name;

        public HealthCheckParameter this[int index] => BaseGet(index) as HealthCheckParameter;

        public new HealthCheckParameter this[string value]
        {
            get { return (HealthCheckParameter)BaseGet(value); }
        }

        protected override string ElementName => "HealthCheckParameter";

        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;

        public IList<HealthCheckParameter> ReturnAll()
        {
            return this.Cast<object>().Cast<HealthCheckParameter>().ToList();
        }
    }
}