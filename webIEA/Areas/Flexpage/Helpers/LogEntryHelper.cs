using Flexpage.Domain.Abstract;
using Flexpage.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Caching;

namespace Flexpage.Helpers
{
    public static class LogEntryHelper
    {
        private const string cacheKeyPrefix = "EntryLogs-";
        private const string topClause = "TOP 10000";

        /// <summary>
        /// Gets logEntry records
        /// </summary>
        public static List<object> GetEntryLogs(IFlexpageRepository repository)
        {
            return LogEntryHelper.GetEntryLogs(repository, x => true);
        }

        /// <summary>
        /// Gets logEntry records
        /// </summary>
        public static List<object> GetEntryLogs(IFlexpageRepository repository, Type mainType = null)
        {
            return GetEntryLogs(repository, x => true, rowCount: 10000, mainType: mainType);
        }

        /// <summary>
        /// Gets logEntry records
        /// </summary>
        public static List<object> GetEntryLogs(IFlexpageRepository repository, Func<LogEntry, bool> predicate, Type mainType = null)
        {
            return GetEntryLogs(repository, predicate, rowCount: 10000, mainType: mainType);
        }

        /// <summary>
        /// Gets logEntry records
        /// </summary>
        public static List<object> GetEntryLogs(IFlexpageRepository repository, Func<LogEntry, bool> predicate, int rowCount, Type mainType = null)
        {
            string cacheKey = LogEntryHelper.cacheKeyPrefix + predicate.GetHashCode();
            if(HttpContext.Current.Cache[cacheKey] != null)
            {
                return (List<object>)HttpContext.Current.Cache[cacheKey];
            }
            else
            {
                List<object> result = new List<object>();

                repository.GetEntityList<LogEntry>(predicate).ForEach(logEntry =>
                {
                    Type tmpType = GetType(logEntry.FullType);

                    if(tmpType != null)
                    {
                        if(mainType == null || (mainType != null && tmpType == mainType) || (mainType.IsAssignableFrom(tmpType)))
                        {
                            object tmpObj = Activator.CreateInstance(tmpType);
                            if(tmpObj != null)
                            {
                                LogEntryHelper.InvokeCTor(tmpObj, logEntry.ID, logEntry.Entry, logEntry.FullType);

                                result.Add(tmpObj);
                            }
                        }
                    }
                });


                HttpContext.Current.Cache.Insert(cacheKey, result, null, DateTime.Now.AddMinutes(180), Cache.NoSlidingExpiration);
                return result;
            }
        }

        /// <summary>
        /// Gets logEntry records
        /// </summary>
        public static void ClearEntryLogs(string whereClause)
        {
            string cacheKey = LogEntryHelper.cacheKeyPrefix + whereClause;
            if(HttpContext.Current != null && HttpContext.Current.Cache[cacheKey] != null)
            {
                HttpContext.Current.Cache.Remove(cacheKey);
            }
        }

        public static Type GetType(string typeName)
        {
            //var type = (from assembly in AppDomain.CurrentDomain.GetAssemblies() from type in assembly.GetTypes() where type.Name == page.RendererType && type.GetMethods().Any(m => m.Name == "GeneratePageBlocks") select type).FirstOrDefault();  
            var type = Type.GetType(typeName);
            if(type != null) return type;
            foreach(var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = a.GetType(typeName);
                if(type != null)
                    return type;
            }
            return null;
        }

        /// <summary>
        /// Gets list of types in result set that returns by GetEntryLogs
        /// </summary>
        public static List<Type> GetEntryLogTypes(IFlexpageRepository repository)
        {
            return LogEntryHelper.GetEntryLogTypes(repository, x => true, typeof(LogEntry));
        }

        /// <summary>
        /// Gets list of types in result set that returns by GetEntryLogs
        /// </summary>

        public static List<Type> GetEntryLogTypes(IFlexpageRepository repository, Func<LogEntry, bool> predicate)
        {
            return LogEntryHelper.GetEntryLogTypes(repository, predicate, typeof(LogEntry));
        }

        /// <summary>
        /// Gets list of types in result set that returns by GetEntryLogs
        /// </summary>
        public static List<Type> GetEntryLogTypes(IFlexpageRepository repository, Type mainType)
        {
            return LogEntryHelper.GetEntryLogTypes(repository, x => true, mainType);
        }

        public static List<Type> GetEntryLogTypes(IFlexpageRepository repository, Func<LogEntry, bool> predicate, Type mainType)
        {
            List<Type> result = new List<Type>();

            repository.GetEntityList<LogEntry>(predicate).GroupBy(x => x.FullType).Select(x => x.FirstOrDefault()).ToList().ForEach(logEntry =>
            {
                var type = GetType(logEntry.FullType);
                if(mainType.IsAssignableFrom(type)) //if(type.IsAssignableFrom(mainType))
                {
                    result.Add(type);
                }
                else
                {
                    result.Add(mainType);
                }
            });

            return result;
        }

        /// <summary>
        /// Invoke constructor of object with params
        /// </summary>
        public static void InvokeCTor(object instance, int ID, string entry, string fullType)
        {
            Type[] types = { typeof(int), typeof(string), typeof(string) };
            ConstructorInfo ci = instance.GetType().GetConstructor(types);
            if(ci != null)
            {
                object[] parameters = { ID, entry, fullType };
                ci.Invoke(instance, parameters);
            }
        }


        /// <summary>
        /// Gets properties of LogEntry (default/base class)
        /// </summary>
        public static List<PropertyInfo> GetDefaultProperties()
        {
            return typeof(LogEntry).GetProperties().ToList();
        }
        public static List<PropertyInfo> GetDefaultProperties(Type type)
        {
            return type.GetProperties().ToList();
        }
        public static List<PropertyInfo> GetDefaultProperties(string typeName)
        {
            return GetType(typeName).GetProperties().ToList();
        }
    }
}