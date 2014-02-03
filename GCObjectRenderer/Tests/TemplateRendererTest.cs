using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace GCObjectRenderer.Tests
{
    public class TemplateRendererTest : BaseTest
    {
        public class TestClass1
        {
        }

        public class TestClass2
        {
            public int A = 20;
            public string AGUGU = "augugugugu";
            public string GetBlaBlaBla()
            {
                return "bla bla bla";
            }
            public string ZZZ
            {
                get
                {
                    return "ZZZ";
                }
            }

            public int[] IntArray = {20, 30, 40, 50, 60};

            public string Concat(string xx, string yy)
            {
                return xx + "-!-" + yy;
            }

            public override string ToString()
            {
                return " TEST OBJECT";
            }
        }

        [Test]
        public void TestSimpleRender()
        {
            
            List<string> ps = new List<string>();

            object m = new TestClass1();
            TemplateRenderer renderer = new TemplateRenderer("AGUGU");
            Console.WriteLine(renderer.Render(m, ps));
            
            object model = new TestClass2();

            renderer = new TemplateRenderer(" ^ ---- ^.A ----\n ^.AGUGU ==== ^.ZZZ \n ^.GetBlaBlaBla()  ^.Concat(qwerty,ytrewq) \n");
            Console.WriteLine(renderer.Render(model, ps));

            renderer = new TemplateRenderer("-- ^.AGUGU.ToString() -- \n");
            Console.WriteLine(renderer.Render(model, ps));

            renderer = new TemplateRenderer("-- ^.AGUGU.ToString().ToString() -- \n");
            Console.WriteLine(renderer.Render(model, ps));

            renderer = new TemplateRenderer("-- ^.AGUGU{ XXX YYY ZZZ } -- \n");
            Console.WriteLine(renderer.Render(model, ps));

            renderer = new TemplateRenderer("-- ^.AGUGU{ ^.ToString()== } -- \n");
            Console.WriteLine(renderer.Render(model, ps));

            renderer = new TemplateRenderer("-- ^.Concat(^.AGUGU,^.A.ToString()){^} -- \n");
            Console.WriteLine(renderer.Render(model, ps));

            renderer = new TemplateRenderer("-- ^.Concat(^.AGUGU,^.A.ToString()){ ^.IndexOf(2) } -- \n");
            Console.WriteLine(renderer.Render(model, ps));

            renderer = new TemplateRenderer("-- ^.IntArray[]{^ } -- \n");
            Console.WriteLine(renderer.Render(model, ps));

            renderer = new TemplateRenderer("-- ^.IntArray[^>40]{^ } -- \n");
            Console.WriteLine(renderer.Render(model, ps));

            renderer = new TemplateRenderer("-- ^.IntArray[^>=40]{^ } -- \n");
            Console.WriteLine(renderer.Render(model, ps));

            renderer = new TemplateRenderer("-- ^.IntArray[^.^<40]{^ } -- \n");
            Console.WriteLine(renderer.Render(model, ps));

            renderer = new TemplateRenderer("-- ^.IntArray[^<=40]{^ } -- \n");
            Console.WriteLine(renderer.Render(model, ps));

            renderer = new TemplateRenderer("-- ^.IntArray[^=40]{^ } -- \n");
            Console.WriteLine(renderer.Render(model, ps));

            renderer = new TemplateRenderer("-- ^.IntArray[^!=40]{^.^ } -- \n");
            Console.WriteLine(renderer.Render(model, ps));

            renderer = new TemplateRenderer("-- ^.IntArray[^!=40&^.ToString()=30]{^ } -- \n");
            Console.WriteLine(renderer.Render(model, ps));

            renderer = new TemplateRenderer("-- ^.IntArray[^=40|^=20]{^ } -- \n");
            Console.WriteLine(renderer.Render(model, ps));

            renderer = new TemplateRenderer("-- ^.IntArray[^.^>20&^<60]{^ } -- \n");
            Console.WriteLine(renderer.Render(model, ps));

            renderer = new TemplateRenderer("-- ^.IntArray[^<=20|^>=60]{^ } -- \n");
            Console.WriteLine(renderer.Render(model, ps));

            renderer = new TemplateRenderer("-- ^.IntArray[(^<=30|^.^>=50)&(^>=30&^<=50)]{^ } -- \n");
            Console.WriteLine(renderer.Render(model, ps));

            ps.Add("20");
            ps.Add("50");
            renderer = new TemplateRenderer("-- ^.IntArray[^<=@0|^>=@1]{^ } -- \n");
            Console.WriteLine(renderer.Render(model, ps));
        }
    }
}
