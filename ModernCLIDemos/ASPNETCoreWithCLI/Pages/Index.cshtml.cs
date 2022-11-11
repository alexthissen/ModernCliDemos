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

        public IndexModel(ILogger<IndexModel> logger, IFoo foo, IConfiguration config, IOptions<FooOptions> option)
        {
            this.logger = logger;
            logger.LogInformation(foo.DoIt().ToString());
            logger.LogInformation("Bar {bar} and Baz {baz}", option.Value.Bar, option.Value.Baz);
        }

        public void OnGet()
        {

        }
    }
}
