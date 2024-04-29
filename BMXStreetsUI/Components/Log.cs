using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BmxStreetsUI.Components
{
    /// <summary>
    /// For customizing logs
    /// </summary>
    internal class Log
    {
        public static MelonLogger.Instance logger;
        public static void Msg(string msg, bool error = false)
        {
            if(logger != null)
            {
                if(error) logger.Error(msg);
                else logger.Msg(msg);
                return;
            }
            if (error) { UnityEngine.Debug.LogError("StreetsUIERROR: " + msg); } else UnityEngine.Debug.Log("StreetsUI Log: " + msg);
        }
    }
}
