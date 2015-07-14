using System.Collections.Generic;

namespace Chalkable.Web.Models
{
    public class  BaseBoxesViewData
    {
        public bool IsPassing { get; set; }
        public string Title { get; set; }
    }
    public class HoverBoxesViewData<T> : BaseBoxesViewData
    {
        public IList<T> Hover { get; set; }
        protected const int MAX_HOVER_LIST_NUMBER = 4;
    }
}