using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuterApp.Lib
{
    class RuterSettings
    {
        public string GetDeparturesUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["GetDeparturesUrl"]; // Husk å legge til i <appSettings> !
            }
        }
    }
}
