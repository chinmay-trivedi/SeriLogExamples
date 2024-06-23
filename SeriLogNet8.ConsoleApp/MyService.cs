using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeriLogNet8.ConsoleApp
{
    public class MyService : IMyService
    {
        private readonly IConfiguration _config;
        private readonly ILogger _logger;

        public MyService(IConfiguration config, ILogger logger)
        {
            _config = config;
            _logger = logger;
        }

        public void DoSomething()
        {
            _logger.LogError("Exception Occurred during MyService DoSomething().");
        }

       
    }
}
