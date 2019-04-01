using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Altairis.Services.Mailing.SystemNetMail;
using Altairis.Services.Mailing.Templating;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace SampleTemplatedMailing {
    public class Startup {
        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc();
            services.AddPickupFolderMailerService(new PickupFolderMailerServiceOptions {
                PickupFolderName = @"C:\InetPub\MailRoot\pickup"
            });
            services.AddResourceTemplatedMailerService(new ResourceTemplatedMailerServiceOptions {
                ResourceType = typeof(Resources.Mailer)
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            app.UseDeveloperExceptionPage();
            app.UseMvc();
        }
    }
}
