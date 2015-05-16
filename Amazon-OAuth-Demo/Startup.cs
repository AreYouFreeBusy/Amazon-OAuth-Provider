using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Amazon_OAuth_Demo.Startup))]
namespace Amazon_OAuth_Demo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
