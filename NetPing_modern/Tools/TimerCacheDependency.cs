using NetPing.DAL;
using NetPing.Global.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace NetPing.Tools
{
    /// <summary>
    /// Custom cache dependency, update cache every 12 hours or when some will call UpdateCache()
    /// </summary>
    public class TimerCacheDependency : CacheDependency
    {
        //private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly Timer _timer;
        private const int _cacheTimeout = 12 * 60 * 60 * 1000; //12 hours, TODO: Load from config
        
        public static event EventHandler UpdateEvent;
      
        public TimerCacheDependency()
        {
            _timer = new Timer(CheckDependencyCallback, this, _cacheTimeout, _cacheTimeout);
            UpdateEvent += new EventHandler(UpdateCacheEventHandler);
        }

        public static void UpdateCache()
        {
            if (UpdateEvent != null)
                UpdateEvent(null, EventArgs.Empty);
        }
        
        protected void UpdateCacheEventHandler(object sender, EventArgs e)
        {
            NotifyDependencyChanged(sender, e);
            //logger.Info("Reset cache");
        }

        private void CheckDependencyCallback(object sender)
        {
            lock (_timer)
            {
                if (UpdateEvent != null)
                    UpdateEvent(sender, EventArgs.Empty);
            }
        }

        protected override void DependencyDispose()
        {
            if (_timer != null) _timer.Dispose();

            base.DependencyDispose();
        }
    }
}