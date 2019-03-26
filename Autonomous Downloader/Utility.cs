using Newtonsoft.Json;
using System;

namespace Autonomous_Downloader
{
    public static class Utility
    {
        /// <summary>
        /// Quick, hacky way to clone
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toBeCloned"></param>
        /// <returns></returns>
        public static T Clone<T>(T toBeCloned)
        {
            if (toBeCloned == null)
            {
                throw new ArgumentNullException(nameof(toBeCloned));
            }
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(toBeCloned));
        }
    }
}
