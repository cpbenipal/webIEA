using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using Flexpage.Abstract;
using Flexpage.Abstract.DTO;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Flexpage.Models;
using Pluritech.Contact.Abstract;
using Pluritech.Permissions.Abstract;
using Pluritech.Properties.Abstract;
using Pluritech.Settings.Abstract;
using Pluritech.UserProfile.Abstract;

namespace Flexpage.Controllers
{
    public class WebFormBlockController : BasicController
    {
        public readonly IWebFormBlockProvider _webFormBlockProvider;

        public WebFormBlockController(IWebFormBlockProvider webFormBlockProvider, IFlexpageRepository repository, IFlexpageSettings settings, ILocalization localization,
            IPermissionsService permService, IPropertyProvider propertyService, IContactProvider contactProvider, IFlexpage flexpageProcessor, IUserGeneralPermissionProvider userGeneralPermissionProvider)
            : base(repository, settings, localization, permService, propertyService, contactProvider, flexpageProcessor, userGeneralPermissionProvider)
        {
            _webFormBlockProvider = webFormBlockProvider;
        }

        public PartialViewResult WebForm(string alias)
        {
            throw new NotImplementedException();
            // var m = _webFormBlockProvider.Load(alias);
            // return PartialView("~/Areas/Flexpage/Views/Flexpage/BlockWrapper.cshtml", m);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult UpdateWebForm(WebFormModel model, string command, string parameters)
        {
            try
            {
                int v;
                string[] pl;
                if (command.ToLower().Trim() != "save")
                {
                    ModelState.Clear();
                }
                if (!ModelState.IsValid)
                    return GetInvalidModelResponse(ModelState);
                switch (command.ToLower().Trim())
                {
                    case "addfield":
                        // var o = JsonConvert.DeserializeObject<WebFormFieldDropOperation>(parameters);
                        if (String.IsNullOrEmpty(parameters))
                        {

                        }
                        else
                        {
                            pl = (parameters as string).Split(',');
                            if (int.TryParse(pl[0], out v))
                                model.AddField(_repository, v, pl[1], pl[2]);
                        }
                        model.Update(_repository);
                        break;
                    case "addaction":
                        if (String.IsNullOrEmpty(parameters))
                        {

                        }
                        else
                        {
                            pl = (parameters as string).Split(',');
                            if (int.TryParse(pl[0], out v))
                                model.AddAction(_repository, v, pl[1]);
                        }
                        model.Update(_repository);
                        break;
                    case "addsection":
                        if (String.IsNullOrEmpty(parameters))
                        {

                        }
                        else
                        {
                            pl = parameters.Split(',');
                            if (int.TryParse(pl[0], out v))
                                model.AddSection(_repository, v, pl[1]);
                        }
                        model.Update(_repository);
                        break;
                    case "movefield":
                        pl = (parameters as string).Split(',');
                        int tid = 0;
                        if (int.TryParse(pl[0], out v))
                            if (int.TryParse(pl[1], out tid))
                                model.MoveField(v, tid);
                        model.Update(_repository);
                        break;
                    case "deletefield":
                        pl = (parameters as string).Split(',');
                        int si = -1;
                        if (int.TryParse(pl[0], out v) && int.TryParse(pl[1], out si))
                            if (si == -1)
                                model.DeleteField(v);
                            else
                            {
                                var s = model.Sections.FirstOrDefault(e => e.Index == si);
                                if (s != null)
                                    s.DeleteField(v);
                            }
                        model.Update(_repository);
                        break;
                    case "deletesection":
                        if (int.TryParse(parameters, out v))
                            model.DeleteSection(v);
                        model.Update(_repository);
                        break;
                    case "deleteaction":
                        if (int.TryParse(parameters, out v))
                            model.DeleteAction(v);
                        model.Update(_repository);
                        break;
                    case "fieldenumchanged":
                        model.Update(_repository);
                        break;
                    case "fieldtypechanged":
                        model.Update(_repository);
                        break;
                    case "settemplate":
                        model.SetCurrentTemplate(_repository);
                        model.Update(_repository);
                        break;
                    case "setfieldstemplate":
                        if (int.TryParse(parameters, out v))
                            model.FieldsTemplateID = v;
                        model.Update(_repository);
                        break;
                    //case "saveastemplate":
                    //    model.SaveAsTemplate();
                    //    model.Update(_repository);
                    //    break;
                    case "edittemplate":
                        model.EditTemplate(parameters);
                        model.Update(_repository);
                        break;
                    case "changelanguage":
                        model.SetCurrentLanguage(_repository, parameters);
                        model.Update(_repository);
                        break;
                    case "save":
                        model.Update(_repository);
                        model.Apply(_repository);
                        _repository.ApplyChanges();
                        break;
                }
                return GetEditorFor(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("UnhandledException", ex);
                return GetEditorFor(model);
            }
        }

        [HttpPost]
        public ActionResult SubmitWebForm(WebFormModel model, string command, string parameters)
        {
            FormBlock formBlock = _repository.GetByBlockID<FormBlock>(model.ID);
            model.AssignLight(formBlock);
            ModelState.SetModelValue("Name", new ValueProviderResult(formBlock?.Form?.Name, "", System.Globalization.CultureInfo.CurrentCulture));
            ModelState["Name"].Errors.Clear();
            model.ReloadData(_repository, _settings);

            if (command == "fileuploaded")
            {
                return PartialView("~/Areas/Flexpage/Views/Flexpage/WebForm.cshtml", model);
            }
            else
            if (command == "removefile")
            {
                System.IO.File.Delete(HostingEnvironment.MapPath(parameters));
                return PartialView("~/Areas/Flexpage/Views/Flexpage/WebForm.cshtml", model);
            }
            else
            {

                model.ValidateValues(ModelState);
                model.ValidateActions(_repository, _contactProvider, ModelState);
                object r = null;
                if (ModelState.IsValid)
                    try
                    {
                        r = model.ProcessActions(_repository, _contactProvider, ModelState);

                    }
                    catch (Exception ex)
                    {
                        r = null;
                        model.ErrorMessages += "Error processing submitted form. Please try again later.";
                        ModelState.AddModelError("", ex.Message);
                    }
                else
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append("Error submitting form. Please check field values. ");
                    var es = ModelState.Where(e => e.Key == "" && e.Value.Errors.Count > 0).Select(e0 => e0.Value.Errors[0].ErrorMessage);
                    foreach (var e in es)
                        sb.AppendLine(e);
                    model.ErrorMessages = sb.ToString();
                }

                if (!ModelState.IsValid || r == null)
                {
                    return PartialView("~/Areas/Flexpage/Views/Flexpage/WebForm.cshtml", model);
                }
                else
                    return r as ActionResult;
            }
        }

        public PartialViewResult SettingsUpdatedSuccessfully()
        {
            return PartialView("~/Areas/Flexpage/Views/Shared/BlockSettingsSaveSuccessfully.cshtml", "Block settings were successfully saved");
        }

    }
}