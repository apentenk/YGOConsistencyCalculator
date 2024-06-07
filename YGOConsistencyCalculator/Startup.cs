using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(YGOConsistencyCalculator.Startup))]
namespace YGOConsistencyCalculator
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
