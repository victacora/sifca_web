using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PRUEBA.Startup))]
namespace PRUEBA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
