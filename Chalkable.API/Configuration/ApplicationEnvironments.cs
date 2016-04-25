using System.Configuration;

namespace Chalkable.API.Configuration
{
    [ConfigurationCollection(typeof(ApplicationEnvironment), AddItemName = "Add", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class ApplicationEnvironments : ConfigurationElementCollection
    {
        public ApplicationEnvironment this[int index]
        {
            get { return BaseGet(index) as ApplicationEnvironment; }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(ApplicationEnvironment environment)
        {
            BaseAdd(environment);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ApplicationEnvironment();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ApplicationEnvironment)element).Environment;
        }

        public void Remove(ApplicationEnvironment environment)
        {
            BaseRemove(environment.Environment);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }
    }
}