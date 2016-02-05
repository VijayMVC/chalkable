using System;
using System.Collections.Generic;
using System.Linq;
using Chalkable.Common;

namespace Chalkable.StandardImport.Models
{
    public class NSSMappingStorage
    {
        private static HashSet<Pair<Guid, Guid>> set;

        private static NSSMappingStorage storage;
        public static NSSMappingStorage GetStorage()
        {
            return storage ?? (storage = new NSSMappingStorage());
        }

        private NSSMappingStorage()
        {
            if(set == null)
                set = new HashSet<Pair<Guid, Guid>>();
        }

        public void Add(Guid first, Guid second)
        {
            set.Add(new Pair<Guid, Guid>(first, second));
        }

        public bool Contains(Guid first, Guid second)
        {
            return set.Contains(new Pair<Guid, Guid>(first, second));
        }
        
        public IList<Pair<Guid, Guid>> GetByFirst(Guid first)
        {
            return set.Where(x => x.First == first).ToList();
        }

        public IList<Pair<Guid, Guid>> GetBySecond(Guid second)
        {
            return set.Where(x => x.Second == second).ToList();
        }

        public Pair<Guid, Guid> GetElement(Guid first, Guid second)
        {
            throw new NotImplementedException();
           /// return set.Fir
        } 
    }

}
