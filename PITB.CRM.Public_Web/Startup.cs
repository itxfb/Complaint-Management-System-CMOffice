﻿using Microsoft.Owin;
using Owin;
using PITB.CRM.Public_Web;

[assembly: OwinStartup(typeof(Startup))]
namespace PITB.CRM.Public_Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

        }
    }
}