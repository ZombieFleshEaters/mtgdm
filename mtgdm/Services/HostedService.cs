using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using mtgdm.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using mtgdm.Helpers;

namespace mtgdm.Services
{
    public class HostedService : IHostedService
    {
        private readonly IConfiguration _config;
        private readonly IServiceProvider _serviceProvider;

        private Timer _timer;
        public HostedService(IServiceProvider serviceProvider, IConfiguration config)
        {
            _config = config;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(SyncAnalytics, null, TimeSpan.Zero, TimeSpan.FromMinutes(30));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public void SyncAnalytics(object state)
        {
            var analytics = new AnalyticsHelper(_serviceProvider, _config);
            analytics.RefreshAnalyics();
        }

    }
}
