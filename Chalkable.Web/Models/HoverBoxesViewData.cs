using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Chalkable.Web.Models
{
    public class HoverBoxesViewData<T>
    {
        public string Title { get; set; }
        public IList<T> Hover { get; set; }

        protected const int MAX_HOVER_LIST_NUMBER = 4;
    }
}