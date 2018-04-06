#region Namespace Sonata.IdentityModel
//	TODO: comment
#endregion


namespace Sonata.IdentityModel
{
	public class IdentityProvider
	{
		public static void Setup(string symetricSecurityKey)
		{
			ApplicationToken.Setup(symetricSecurityKey);
		}
		
		public static void Reset()
		{
			ApplicationToken.Reset();
		}
	}
}