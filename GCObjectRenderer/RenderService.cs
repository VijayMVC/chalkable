using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GCObjectRenderer
{
    public class RenderService
    {
        private static List<IRendererContainer> containers = new List<IRendererContainer>();
        private static MainRendererContainer mainContainer = new MainRendererContainer();
        static RenderService()
        {
            containers.Add(mainContainer);
        }
        
        private static IRenderer FindRenderer(string name)
        {
            foreach (var container in containers)
            {
                IRenderer res = container.GetRendererByName(name);
                if (res != null)
                    return res;
            }
            throw new Exception("Invalid renderer name");
        }

        public static void RegisterMainRenderer(string name, IRenderer renderer, bool overwrite)
        {
            mainContainer.RegisterRenderer(name, renderer, overwrite);
        }

        public static void AddRendererContainer(IRendererContainer container)
        {
            containers.Add(container);
        }

        public static string Render(string name, object model, List<string> parameters)
        {
            return FindRenderer(name).Render(model, parameters);
        }
    }
}
