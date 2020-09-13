using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using mtgdm.Data;

namespace mtgdm.ConfigurationProvider
{
    public class MTGDMConfigurationProvider : Microsoft.Extensions.Configuration.ConfigurationProvider
    {
        public MTGDMConfigurationProvider(Action<DbContextOptionsBuilder> optionsAction)
        {
            OptionsAction = optionsAction;
        }

        Action<DbContextOptionsBuilder> OptionsAction { get; }

        public override void Load()
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            OptionsAction(builder);

            using (var dbContext = new ApplicationDbContext(builder.Options))
            {
                Data = dbContext.SystemValues.ToDictionary(c => c.Key, c => c.Value);
            }
        }
    }
}
