using System;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using mtgdm.Data;

namespace mtgdm.ConfigurationProvider
{
    public class ConfigurationSource : IConfigurationSource
    {
        private readonly Action<DbContextOptionsBuilder> _optionsAction;

        public ConfigurationSource(Action<DbContextOptionsBuilder> optionsAction)
        {
            _optionsAction = optionsAction;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new MTGDMConfigurationProvider(_optionsAction);
        }
    }
}
