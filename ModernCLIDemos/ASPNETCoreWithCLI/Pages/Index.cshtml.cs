using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCoreWithCLI.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> logger;

        public IndexModel(ILogger<IndexModel> logger, IMigrator migrator, IConfiguration config, IOptions<MigrationOptions> options)
        {
            this.logger = logger; 
            logger.LogInformation(migrator.Migrate(options.Value).ToString());
            logger.LogInformation("Message {Message} and version {Version}", 
                options.Value.Message, options.Value.Version);
        }

        public void OnGet()
        {

        }
    }
}
