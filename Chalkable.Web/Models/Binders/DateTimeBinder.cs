using System;
using System.Globalization;
using System.Web.Mvc;
using Chalkable.Common;

namespace Chalkable.Web.Models.Binders
{
    public class DateTimeBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            try
            {
                string val = GetAttemptedValue(bindingContext);
                if (string.IsNullOrEmpty(val))
                {
                    return null;
                }
                var res = DateTime.Parse(val, CultureInfo.InvariantCulture);
                return res;
            }
            catch (Exception ex)
            {
                string message = string.Format(ChlkResources.ERR_CANT_FIND_VALUE_FOR_QUERY_STRING_PARAM,
                                               bindingContext.ModelName);
                throw new ApplicationException(message, ex);
            }
        }

        private static string GetAttemptedValue(ModelBindingContext bindingContext)
        {
            ValueProviderResult value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
            return value == null ? string.Empty : value.AttemptedValue;
        }
    }

}