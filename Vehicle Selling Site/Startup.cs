using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Vehicle_Selling_Site.Startup))]
namespace Vehicle_Selling_Site
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
