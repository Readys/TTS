using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TTS20.Startup))]
namespace TTS20
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
