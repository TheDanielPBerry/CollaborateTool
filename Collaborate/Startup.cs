using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Collaborate.Startup))]
namespace Collaborate
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
