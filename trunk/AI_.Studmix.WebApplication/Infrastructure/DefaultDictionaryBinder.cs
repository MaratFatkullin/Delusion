using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Mvc;

namespace AI_.Studmix.WebApplication.Infrastructure
{

    public class DefaultDictionaryBinder : DefaultModelBinder
    {
        private readonly IModelBinder _nextBinder;

        public DefaultDictionaryBinder()
            : this(null)
        {
        }

        public DefaultDictionaryBinder(IModelBinder nextBinder)
        {
            _nextBinder = nextBinder;
        }

        private IEnumerable<string> GetValueProviderKeys(ControllerContext context)
        {
            var keys = new List<string>();
            keys.AddRange(context.HttpContext.Request.Form.Keys.Cast<string>());
            keys.AddRange(((IDictionary<string, object>)context.RouteData.Values).Keys);
            keys.AddRange(context.HttpContext.Request.QueryString.Keys.Cast<string>());
            keys.AddRange(context.HttpContext.Request.Files.Keys.Cast<string>());
            return keys;
        }

        private object ConvertType(string stringValue, Type type)
        {
            return TypeDescriptor.GetConverter(type).ConvertFrom(stringValue);
        }

        public override object BindModel(ControllerContext controllerContext,
                                         ModelBindingContext bindingContext)
        {
            Type modelType = bindingContext.ModelType;
            Type idictType = modelType.GetInterface("System.Collections.Generic.IDictionary`2");
            if (idictType != null)
            {
                object result = null;

                Type[] ga = idictType.GetGenericArguments();
                IModelBinder valueBinder = Binders.GetBinder(ga[1]);

                foreach (string key in GetValueProviderKeys(controllerContext))
                {
                    if (key.StartsWith(bindingContext.ModelName + "[",
                                       StringComparison.InvariantCultureIgnoreCase))
                    {
                        int endbracket = key.IndexOf("]", bindingContext.ModelName.Length + 1);
                        if (endbracket == -1)
                            continue;

                        object dictKey;
                        try
                        {
                            dictKey = ConvertType(key.Substring(bindingContext.ModelName.Length + 1,
                                                                endbracket - bindingContext.ModelName.Length -
                                                                1),
                                                  ga[0]);
                        }
                        catch (NotSupportedException)
                        {
                            continue;
                        }

                        var innerBindingContext = new ModelBindingContext
                        {
                            ModelMetadata = ModelMetadataProviders.Current.
                                GetMetadataForType(() => null, ga[1]),
                            ModelName = key.Substring(0, endbracket + 1),
                            ModelState = bindingContext.ModelState,
                            PropertyFilter = bindingContext.PropertyFilter,
                            ValueProvider = bindingContext.ValueProvider
                        };
                        var newPropertyValue = valueBinder.BindModel(controllerContext, innerBindingContext);

                        if (result == null)
                            result = CreateModel(controllerContext, bindingContext, modelType);

                        if (!(bool)idictType.GetMethod("ContainsKey").Invoke(result, new[] { dictKey }))
                            idictType.GetProperty("Item").SetValue(result, newPropertyValue, new[] { dictKey });
                    }
                }

                return result;
            }

            if (_nextBinder != null)
            {
                return _nextBinder.BindModel(controllerContext, bindingContext);
            }

            return base.BindModel(controllerContext, bindingContext);
        }
    }
}