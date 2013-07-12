using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCObjectRenderer
{
    public interface IRendererContainer
    {
        IRenderer GetRendererByName(string name);
    }

    public class MainRendererContainer : IRendererContainer
    {
        private Dictionary<string, IRenderer> renderers = new Dictionary<string, IRenderer>();
        public void RegisterRenderer(string name, IRenderer renderer, bool overwrite)
        {
            if (renderers.ContainsKey(name))
            {
                if (overwrite)
                    renderers.Remove(name);
                else
                    throw new Exception("Renderer with such name already exists");
            }
            renderers.Add(name, renderer);
        }

        public IRenderer GetRendererByName(string name)
        {
            if (renderers.ContainsKey(name))
                return renderers[name];
            return null;
        }
    }
}
