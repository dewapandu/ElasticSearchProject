using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Person.Startup))]
namespace Person
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
