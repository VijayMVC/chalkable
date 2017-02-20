using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chalkable.Web.Models
{
    public class AppNotificationInput
    {
        public int PersonId { get; set; }
        [AllowHtml]
        public string HtmlText { get; set; }
    }
}