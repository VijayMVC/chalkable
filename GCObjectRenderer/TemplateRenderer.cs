using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GCObjectRenderer
{
    /// <summary>
    /// @ - parameter
    /// ^ - self
    /// 
    /// 
    /// </summary>
    public class TemplateRenderer : IRenderer
    {
        private class RObject
        {
            private object innerModel;
            private string conditions;
            private string body;
            private bool hasConditions = false;
            private bool hasBody = false;

            public RObject(object innerModel, string conditions, string body)
            {
                this.innerModel = innerModel;
                if (conditions != null)
                {
                    this.conditions = conditions;
                    hasConditions = true;
                }
                if (body != null)
                {
                    this.body = body;
                    hasBody = true;
                }
                
            }
            public string Body
            {
                get
                {
                    return body;
                }
            }

            public bool HasBody
            {
                get
                {
                    return hasBody;
                }
            }

            public Condition Conditions
            {
                get
                {
                    if (string.IsNullOrEmpty(conditions.Trim()))
                        return null;
                    return ConditionBuilder.BuildCondition(conditions);
                }
            }

            public bool HasCondition
            {
                get
                {
                    return hasConditions;
                }
            }

            public object InnerModel
            {
                get
                {
                    return innerModel;
                }
            }
        }

        private string renderTemplate;
        public TemplateRenderer(string template)
        {
            renderTemplate = template;
        }

        public string Render(object model, List<string> parameters)
        {
            string template = ReplaceParams(renderTemplate, parameters);
            return RenderTemplate(template, model);
        }

        private string ReplaceParams(string template, List<string> parameters)
        {
            if (parameters != null)
                for (int i = parameters.Count - 1; i >= 0; i-- )
                {
                    string p = "@" + i;
                    template = template.Replace(p, parameters[i]);
                }
            return template;
        }

        private string RenderTemplate(string template, object model)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < template.Length;)
            {
                if (template[i] == '^')
                {
                    RObject obj = SelectObject(template, ref i, model);
                    result.Append(RenderObject(obj));
                }
                else
                {
                    result.Append(template[i]);
                    i++;
                }
            }
            return result.ToString();
        }

        private string RenderObject(RObject obj)
        {
            object current = obj.InnerModel;
            if (obj.HasCondition)
            {
                IEnumerable en;
                if (current is IEnumerable)
                {
                    en = current as IEnumerable;
                    
                }
                else
                {
                    List<object> list = new List<object>();
                    list.Add(current);
                    en = list;
                }
                StringBuilder res = new StringBuilder();
                foreach (var o in en)
                    if (CheckCondition(o, obj.Conditions))
                    {
                        if (obj.HasBody)
                            res.Append(RenderTemplate(obj.Body, o));
                        else
                            return o.ToString();
                    }
                        
                return res.ToString();
            }
            if (obj.HasBody)
                return RenderTemplate(obj.Body, current);
            return current.ToString();
                
        }

        

        private RObject SelectObject(string template, ref int i, object model)
        {
            object innerModel = ReflectionHelper.ReadObject(template, ref i, model);
            string conditions = SelectObjectConditions(template, ref i);
            string body = SelectObjectBody(template, ref i);
            RObject result = new RObject(innerModel, conditions, body);
            return result;
        }

        private string SelectObjectBody(string template, ref int i)
        {
            string result = BraceHelper.SelectBracesContent(template, ref i, '{', '}');
            return result;
        }

        private string SelectObjectConditions(string template, ref int i)
        {
            return BraceHelper.SelectBracesContent(template, ref i, '[', ']');
        }

        private bool CheckCondition(object model, Condition conditions)
        {
            if (conditions == null)
                return true;
            return conditions.Check(model);
        }
    }
}
