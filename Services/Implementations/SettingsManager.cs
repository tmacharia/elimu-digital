using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public class SettingsManager : ISettingsManager
    {
        private readonly IConfigurationSection _config;

        public SettingsManager(IConfigurationSection config)
        {
            _config = config;
        }

        public bool SendEmailOnError
        {
            get
            {
                string val = _config["SendEmailOnError"];

                if (string.IsNullOrWhiteSpace(val))
                {
                    throw new KeyNotFoundException("SendEmailOnError setting was not found in settings section in configuration.");
                }

                if(val == "True" || val == "true")
                {
                    return true;
                }
                else if(val == "False" || val == "false")
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool LogError
        {
            get
            {
                string val = _config["LogError"];

                if (string.IsNullOrWhiteSpace(val))
                {
                    throw new KeyNotFoundException("LogError setting was not found in settings section in configuration.");
                }

                if (val == "True" || val == "true")
                {
                    return true;
                }
                else if (val == "False" || val == "false")
                {
                    return false;
                }
                else
                {
                    return false;
                }
            }
        }

        public string DevEmail
        {
            get
            {
                string val = _config["DevEmail"];

                if (string.IsNullOrWhiteSpace(val))
                {
                    throw new KeyNotFoundException("DevEmail setting was not found in settings section in configuration.");
                }

                return val;
            }
        }
    }
}
