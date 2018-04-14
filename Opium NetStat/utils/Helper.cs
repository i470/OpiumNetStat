using System;
using System.Collections.Generic;
using System.Windows;


namespace Opium_NetStat.utils
{
    
    public static class Helper
    {
        /// <summary>
        ///     Retrieve the parameter value from the dictionary
        /// </summary>
        
        public static T ParameterValue<T>(this IDictionary<string, object> parameters, string name)
        {
            if (parameters.ContainsKey(name) && parameters[name] is T)
            {
                return (T)parameters[name];
            }

            return default(T);
        }
        
        public static void ExecuteOnUI(Action action)
        {
            var dispatcher = Application.Current.Dispatcher;
            if (dispatcher.CheckAccess())
                action();
            else dispatcher.BeginInvoke(action);
        }
    }
}