using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autonomous_Downloader.Autonomous_x
{
    public class PlayGroup
    {
        public ObservableCollection<AutonomousPlay> AutonomousPlays = new ObservableCollection<AutonomousPlay>();

        public static PlayGroup Load(String filepath)
        {
            PlayGroup retval = null;

            using (StreamReader sr = new StreamReader(filepath))
            {
                String json;

                json = sr.ReadToEnd();
                retval = JsonConvert.DeserializeObject<PlayGroup>(json);
            }

            return retval;
        }

        public void Save(String filepath)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filepath))
                {
                    String json = JsonConvert.SerializeObject(this, Formatting.Indented);
                    sw.Write(json);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }
    }
}
