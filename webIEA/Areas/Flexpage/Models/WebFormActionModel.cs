using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Flexpage.Abstract;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Domain.Enum;
using Newtonsoft.Json;
using Enum = Flexpage.Domain.Entities.Enum;


namespace Flexpage.Models
{

    public class WebFormActionModel : ViewModel
    {
        public bool ReadOnly { get; set; }
        public bool Selected { get; set; }
        public int Index { get; set; }
        public int FormID { get; set; }
        public WebFormModel Form { get; set; }
        public int ActionID { get; set; }
        public string Function { get; set; }
        public string Description { get; set; }
        public string Parameter { get; set; }
        public bool IsVoid { get; set; }

        public void Apply(FormActionLink target)
        {
            target.ActionID = ActionID;
            target.Order = Index;
            target.Parameter = Parameter;
            // target.FormID = FormID;
        }

        public override object Apply(IFlexpageRepository repository, params object[] args)
        {
            if (args[0] is FormActionLink)
            {
                var l = args[0] as FormActionLink;
                Apply(l);
                return l;
            }
            return base.Apply(repository, args);
        }

        public override void Assign(object source, params object[] args)
        {
            try
            {
                base.Assign(source, args);
                if (source is FormActionLink)
                {
                    var l = source as FormActionLink;
                    ID = l.ID;
                    FormID = l.FormID;
                    ActionID = l.ActionID;
                    ///2do: remove after test finished
                    //if (testRnd.Next(0, 13) == 0)
                    //    throw new Exception("This is form action test exception");

                    Function = l.FormAction.Function;
                    ///2do: remove after test finished
                    //if (testRnd.Next(0, 13) == 0)
                    //    throw new Exception("This is form field action exception");

                    Description = l.FormAction.Description;
                    Index = l.Order;
                    Parameter = l.Parameter;
                }
                else if (source is FormAction)
                {
                    var a = source as FormAction;
                    ID = -1;
                    ActionID = a.ID;
                    Index = 100500;
                    Function = a.Function;
                    Description = a.Description;
                }
            }
            catch
            {
                HasErrors = true;
                Function = "";
                Description = "";
            }
        }
        //посмотреть, как работает блок ContactUS в старом Flexpage2, и реализовать его 
        //VALIDATE
        //SAVETODB
        //CALLCONTROLLERACTION(url)

        public WebFormActionModel(IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {

        }


        public WebFormActionModel(object source, IFlexpageSettings settings, Abstract.IFlexpage flexpage) : base(settings, flexpage)
        {
            Assign(source);
        }

        //public dynamic TryParseJson(string str, dynamic definition)
        //{
        //    definition = new { AddressField = "" };
        //    if (str == null)
        //        return null;
        //    bool success = true;

        //    var settings = new JsonSerializerSettings
        //    {

        //        Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
        //        MissingMemberHandling = MissingMemberHandling.Error
        //    };
        //    dynamic result = JsonConvert.DeserializeAnonymousType(str, definition, settings);
        //    if (success)
        //        return result;
        //    return null;
        //}

        //public dynamic ParseParameter(dynamic definition, out bool correctJSON)
        //{
        //    string regexPattern = "\"([^\"]+)\":"; // the "propertyName": pattern
        //    var p = Regex.Replace(Parameter, regexPattern, "$1:");
        //    correctJSON = false;
        //    if (definition == null)
        //        return null;

        //    dynamic result = TryParseJson(p, definition);
        //    if (result == null)
        //    {
        //        Type targetType = definition.GetType();
        //        try
        //        {
        //            result = new ExpandoObject();
        //            var propInfo = targetType.GetProperties().FirstOrDefault();
        //            ((IDictionary<string, object>)result).Add(propInfo.Name, Parameter);
        //            return result;
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    }
        //    return result;
        //}

        public dynamic ParseParameter(dynamic definition, out bool correctJSON)
        {
            correctJSON = false;
            if (definition == null)
                return null;
            if (Parameter == null)
                return definition;
            var success = true;
            var settings = new JsonSerializerSettings
            {

                Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error
            };
            var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(Parameter, settings);
            correctJSON = success;
            if (!correctJSON)
                dic = new Dictionary<string, object>();

            ExpandoObject result = new ExpandoObject();
            Type targetType = definition.GetType();
            var propInfos = targetType.GetProperties();
            foreach (var prop in propInfos)
            {
                object val = prop.GetValue(definition);
                if (dic.ContainsKey(prop.Name))
                    try {
                        val = Convert.ChangeType(dic[prop.Name], val.GetType());
                    }
                    catch
                    {

                    }
                else if (correctJSON)
                {
                }
                else if (Parameter.Contains('{') || Parameter.Contains('}')) // check for broken json
                {
                }
                else
                    val = Parameter;
                ((IDictionary<string, object>)result).Add(prop.Name, val);
            }

            return result;
        }
    }

}