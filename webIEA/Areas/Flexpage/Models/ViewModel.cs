using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using Flexpage.Abstract;
using Newtonsoft.Json.Linq;

namespace Flexpage.Models
{
    public class ViewModel
    {
        /// <summary>
        /// 2DO: Must be removed. Used to test form editor exceptions sustainability 
        /// </summary>
        // public static Random testRnd = new Random();

        public bool HasErrors { get; set; } = false;

        [JsonIgnore]
        public int ID { get; set; }
        [JsonIgnore]
        public bool IsStatic { get; set; }
        public bool IsEdit { get; set; } = true;
        [JsonIgnore]
        public bool AdminMode { get; set; }
        public bool ShowAdminControls { get; set; }
        public static string NewAlias { get { return "#create new#"; } }
        [JsonIgnore]
        protected IFlexpageSettings _settings;
        [JsonIgnore]
        public IFlexpageSettings Settings
        {
            get
            {
                return _settings;
            }

            set
            {
                _settings = value;
            }
        }

        [JsonIgnore]
        protected IFlexpage _flexpageProcessor;
        [JsonIgnore]
        public IFlexpage FlexpageProcessor
        {
            get
            {
                return _flexpageProcessor;
            }

            set
            {
                _flexpageProcessor = value;
            }
        }

        protected ViewModel(IFlexpageSettings settings, IFlexpage flexpageProcessor)
        {
            _settings = settings;
            if (_settings != null)
                AdminMode = _settings.IsCurrentPageAdmin();
            _flexpageProcessor = flexpageProcessor;
        }

        [JsonIgnore]
        public string EditorPostfix { get; set; }

        public static string ExtractShortTypeName(string text)
        {
            //return text;
            string[] ss = text.Split(',');
            string s = ss[0].Trim();
            int si = s.LastIndexOf('.');
            /* if (si < 0)
                si = 0; */
            return s.Substring(si + 1);
        }

        public static Dictionary<string, object> ParseJSON(string text)
        {
            if(String.IsNullOrEmpty(text))
                return new Dictionary<string, object>();
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
        }

        public static string ToJSON(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T FromJSON<T>(string text)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(text);
            }
            catch
            {
                return default(T);
            }
        }

        public  static bool IsValidJson(string strInput)
        {
            if(strInput != null)
            {
                strInput = strInput.Trim();
                if((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                    (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
                {
                    try
                    {
                        var obj = JToken.Parse(strInput);
                        return true;
                    }
                    catch(JsonReaderException jex)
                    {
                        //Exception in parsing json
                        Console.WriteLine(jex.Message);
                        return false;
                    }
                    catch(Exception ex) //some other exception
                    {
                        Console.WriteLine(ex.ToString());
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Extracts property from provided Dictionary if one exists or returns null
        /// </summary>
        /// <param name="dic">Dictionary to search</param>
        /// <param name="key">Key to lookup</param>
        public static string ExtractProperty(Dictionary<string, object> dic, string key)
        {
            if(dic.ContainsKey(key))
            {
                return dic[key] as string;
            }
            return null;
        }

        public static string ExtractFirsOrDefaultProperty(Dictionary<string, object> dic, string key)
        {
            string value = null;
            if(dic.ContainsKey(key))
            {
                value = dic[key] as string;
            }
            else if(dic.Keys.Count > 0)
            {
                value = dic.FirstOrDefault().Value as string;
            }

            return value;
        }

        public static string GetNumberedFilePath(string fullPath)
        {
            int count = 1;

            string fileNameOnly = System.IO.Path.GetFileNameWithoutExtension(fullPath);
            string extension = System.IO.Path.GetExtension(fullPath);
            string path = System.IO.Path.GetDirectoryName(fullPath);
            string newFullPath = fullPath;

            while(System.IO.File.Exists(newFullPath))
            {
                string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                newFullPath = System.IO.Path.Combine(path, tempFileName + extension);
            }
            return newFullPath;
        }

        public static string GetNextAvailableFilePath(string fullPath)
        {
            if(!System.IO.File.Exists(fullPath)) return fullPath;

            string alternateFilename;
            int fileNameIndex = 1;
            do
            {
                fileNameIndex += 1;
                alternateFilename = CreateNumberedFilePath(fullPath, fileNameIndex);
            } while(System.IO.File.Exists(alternateFilename));

            return alternateFilename;
        }

        private static string CreateNumberedFilePath(string fullPath, int number)
        {
            string plainName = System.IO.Path.GetFileNameWithoutExtension(fullPath);
            string extension = System.IO.Path.GetExtension(fullPath);
            return string.Format("{0}{1}{2}", plainName, number, extension);
        }

        public List<Type> GetAllDerivedEntities(Type type)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                 .Where(x => type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .ToList();
        }

        /// <summary>
        /// Creates view model of given class name
        /// </summary>
        /// <param name="className">Class name</param>
        /// <returns>Model instance</returns>
        //public static ViewModel Create(string className, Flexpage.Abstract.IFlexpageSettings settings)
        //{
        //    Type t = Type.GetType("Flexpage.Models." + className);
        //    if(t != null)
        //        return Activator.CreateInstance(t, settings) as ViewModel;
        //    return null;
        //}

        public static ViewModel Create(string className, Flexpage.Abstract.IFlexpageSettings settings, IFlexpage flexpageProcessor)
        {
            Type t = Type.GetType("Flexpage.Models." + className);
            if (t != null)
            {
                //if (Infrastructure.FlexpageModelBuilder.requiresFlexpageProcessor(t))
                //    return Activator.CreateInstance(t, settings, flexpageProcessor) as ViewModel;
                return Activator.CreateInstance(t, settings, flexpageProcessor) as ViewModel;
            }
            return null;
        }

        /// <summary>
        /// Assigns content from source (domain entity)
        /// </summary>
        /// <param name="source">FlexPage.Domain entity</param>
        /// <param name="args">Various arguments( alias, for example). Detailed description in implementations</param>
        public virtual void Assign(object source, params object[] args)
        {

        }

        /// <summary>
        /// Assigns default values to the new created block
        /// </summary>
        /// <param name="args">Various arguments( alias, for example). Detailed description in implementations</param>
        public virtual void AssignDefaultValues(params object[] args)
        {
            IsEdit = true;
        }

        /// <summary>
        /// Loads content from DB by provided key. Should be implemented in every class inherited
        /// </summary>
        /// <param name="repository">Flexpage repository with CMS data</param>
        /// <param name="needToLoadContent">If true, content of complex blocks (i.e. BlockList)</param>
        public virtual void Load(IFlexpageRepository repository, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        {
        }

        //public virtual void Load(IFlexpageRepository repository, IFlexpage flexpageProcessor, BlockCommandModel proto, string title = "", bool needToLoadContent = true)
        //{
        //}
        /// <summary>
        /// Creates or loads domain entity from DB, updates it and saves it back
        /// If model identifier(alias?) == 0, == repository.CreateNewAlias ? then create new entity, otherwize - update existing one.
        /// </summary>
        public virtual object Apply(IFlexpageRepository repository, params object[] args)
        {
            return null;
        }

        /// <summary>
        /// Deletes bound domain entity from database
        /// </summary>
        public virtual void Delete(IFlexpageRepository repository)
        {

        }

        /// <summary>
        /// Performs internal view model updates (for example, updates currently edited text localization in cms text model, returned from client)
        /// </summary>
        public virtual void Update()
        {

        }

        public virtual string ToJson()
        {
            return ToJSON(this);
        }

        public Flexpage.Domain.Entities.BlockType GetBlockType(IFlexpageRepository repository)
        {
            string tn = this.GetType().Name.Replace("Model", "");
            return repository.GetEntityList<Domain.Entities.BlockType>().FirstOrDefault(e => e.Name == tn);
        }

        public virtual void FillViewData(ViewDataDictionary viewData, IFlexpageRepository repository,string Title="")
        {
        }

        private HashSet<string> GetLangs(List<LocalizedStringModel> localizedStringModels = null,
            List<LocalizedTextModel> localizedTextModels = null)
        {
            HashSet<string> descrLangs = new HashSet<string>();
            if (localizedStringModels != null)
            {
                localizedStringModels.ForEach(lsm =>
                {
                    if (lsm != null)
                    {
                        foreach (string key in lsm.Localizations.Keys.Except(descrLangs))
                        {
                            if (!string.IsNullOrWhiteSpace(lsm.Localizations[key]))
                            {
                                descrLangs.Add(key);
                            }
                        }
                    }
                });
            }
            if (localizedTextModels != null)
            {
                localizedTextModels.ForEach(lsm =>
                {
                    if (lsm != null)
                    {
                        foreach (string key in lsm.Texts.Keys.Except(descrLangs))
                        {
                            if (!string.IsNullOrWhiteSpace(lsm.Texts[key]?.Text))
                            {
                                descrLangs.Add(key);
                            }
                        }
                    }
                });
            }
            return descrLangs;
        }

        protected LanguageSelectorModel GetLanguageSelector(string CurrentLangCode,
             List<LocalizedStringModel> localizedStringModels = null,
            List<LocalizedTextModel> localizedTextModels = null, string FunctionName = "fp_changeLanguage")
        {
            var  _languageSelector = new LanguageSelectorModel(_settings, _flexpageProcessor)
            {
                CurrentLangCode = CurrentLangCode,
                FunctionName = FunctionName,
                LangCodes = GetLangs(localizedStringModels, localizedTextModels)
            };
            return _languageSelector;
        }
    }
}