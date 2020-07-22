using Microsoft.AspNetCore.Mvc.Testing;

namespace ContactlessEntry.Cloud.UnitTests.Utilities
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
    }
}
