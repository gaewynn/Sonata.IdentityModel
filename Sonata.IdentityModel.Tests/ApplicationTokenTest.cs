using Xunit;

namespace Sonata.IdentityModel.Tests
{
	public class ApplicationTokenTest
    {
        [Fact]
        public void GenerateToken()
        {
			IdentityProvider.Setup("ebe57650-2364-481f-b157-c771a97ee3d6");
	        var applicationToken = new ApplicationToken("app_key", "use");
	        var serializedToken = applicationToken.WriteToken();

	        var res = ApplicationToken.ReadToken(serializedToken);
        }
    }
}