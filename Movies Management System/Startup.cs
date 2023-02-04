using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Movies_Management_System.Startup))]
namespace Movies_Management_System
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
