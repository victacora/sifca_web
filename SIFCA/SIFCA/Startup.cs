using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SIFCA.Startup))]
namespace SIFCA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
