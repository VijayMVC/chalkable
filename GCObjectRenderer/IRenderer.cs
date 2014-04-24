using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCObjectRenderer
{
    public interface IRenderer
    {
        string Render(object model, List<string> parameters);
    }
}
