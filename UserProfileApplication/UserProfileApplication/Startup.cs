using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UserProfileApplication.Startup))]
namespace UserProfileApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
