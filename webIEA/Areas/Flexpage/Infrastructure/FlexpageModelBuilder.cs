using DevExpress.Web.Mvc;
using Flexpage.Abstract;
using Flexpage.Helpers;
using Flexpage.Models;
using Pluritech.Services;
using System;
using System.Web.Mvc;

namespace Flexpage.Infrastructure
{
    public class FlexpageModelBuilder : DevExpressEditorsBinder
    {
        protected override object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            if (modelType.IsSubclassOf(typeof(ViewModel)))
            { 
                return ViewModel.Create(modelType.Name, Flexpage.Helpers.FlexpageSettings.Instance, DependencyResolver.Current.GetService<IFlexpage>());
            }
            return base.CreateModel(controllerContext, bindingContext, modelType);
        }

        public static bool requiresFlexpageProcessor(Type modelType)
        {
            foreach (var item in modelType.GetConstructors())
            {
                foreach (var parameter in item.GetParameters())
                {
                    if (parameter.ParameterType == typeof(IFlexpage))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}