using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GitHubUserDetails.Startup))]
namespace GitHubUserDetails
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //ConfigureAuth(app);
        }
    }
}
