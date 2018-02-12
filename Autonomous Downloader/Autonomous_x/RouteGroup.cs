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
    /// <summary>
    /// 
    /// </summary>
    ///
    /// General overview of the data structure:
    /// 
    /// RouteGroup - the top level container for all routes.
    ///     The RouteGroup contains a list of zero or more AutonomousRoute objects
    ///     
    ///     The RouteGroup is loaded or stored into the JSON file when the user 
    ///     clicks the load or save button.    
    /// 
    /// AutonomousRoute - this specifies a single route.
    ///     The AutonomousRoute contains a list of zero or more Command objects (route steps).
    /// Command - the steps that a route performs
    ///     The command contains a list of zero or more ParameterInstances.
    ///    
    /// 
    /// CommandTemplate - When an instance of a Command is created it is tied to a template
    ///     that identifies its name and the parameters that it takes.
    ///     
    ///     The ProgramPanel can either create a standard set of CommandTemplates or if the "commands.json" 
    ///     is found will load them from that file.
    ///
    public class RouteGroup
    {
        public ObservableCollection<AutonomousRoute> AutonomousModes = new ObservableCollection<AutonomousRoute>();

        /// <summary>
        /// Kick off the process to load the data structure.
        /// </summary>
        /// 
        /// See Save for a little bit more information on the process.
        /// See also the JsonConvert class for more detailed information
        /// on its workings.
        /// 
        /// <param name="filepath">The name of the file to load</param>
        /// 
        /// <returns>Returns true if the load is successful, otherwise false is
        /// returned.</returns>
        /// 
        public static RouteGroup Load(String filepath)
        {
            RouteGroup retval = null;

            using (StreamReader sr = new StreamReader(filepath))
            {
                String json;

                json = sr.ReadToEnd();
                retval = JsonConvert.DeserializeObject<RouteGroup>(json);
            }

            return retval;
        }

        /// <summary>
        /// Kick off the process to save the data structure.
        /// </summary>
        /// 
        /// The JsonConvert.SerializeObject will start at 'this' object
        /// and save each of its data members. The SerializeObject 
        /// method is smart enough that it can look at the class 
        /// structure and figure out what it needs to save without
        /// the programmer having to add code to define the structure.
        /// 
        /// Although the exact process of of how the system figures out
        /// the data structure isn't known to me (the application dev)
        /// it would have to include some means of collecting runtime
        /// type information (RTTI) an examination of the data members
        /// and then following those members down.
        /// 
        /// <param name="filepath">The name of the file to save</param>
        /// 
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
