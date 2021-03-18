using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using HealthMonitor.Domain.Configuration.Interfaces;

namespace HealthMonitor.Domain.Configuration
{
    public class RunTimes : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement() => new RunTime();

        protected override object GetElementKey(ConfigurationElement element) => ((RunTime)element).Value;

        public RunTime this[int index] => BaseGet(index) as RunTime;

        public new RunTime this[string value]
        {
            get { return (RunTime)BaseGet(value); }
        }

        protected override string ElementName => "RunTime";

        public override ConfigurationElementCollectionType CollectionType => ConfigurationElementCollectionType.BasicMap;

        public IList<RunTime> ReturnAll()
        {
            return this.Cast<object>().Cast<RunTime>().ToList();
        }

    }
}