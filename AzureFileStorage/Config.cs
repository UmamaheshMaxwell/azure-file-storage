using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace AzureFileStorage
{
    public class Config
    {
        private readonly IConfiguration _config;
        public Config(IConfiguration config)
        {
            _config = config;
        }

        public string getValue(string key)
        {
           return _config[key];
        }
    }
}
