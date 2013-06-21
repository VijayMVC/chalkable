using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chalkable.Data.Master.Model
{
    public class ImportSystemType
    {
        public ImportSystemTypeEnum Type { get; set; }
        public string Name { get; set; }
    }

    public enum ImportSystemTypeEnum
    {
        None = -1,
        Lighthouse = 0,
        InfiniteCampus = 1,
        Esd = 2,
        Sti = 3
    }
}
