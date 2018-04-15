using System;

namespace Sonata.IdentityModel.Tests.Fixtures
{
	public class IdentityProviderFixture : IDisposable
	{
		#region Constants

		public const string SecurityKey = "36f986d1-cb36-4393-874d-99807ec77846";

		#endregion

		#region Constructors

		public IdentityProviderFixture()
		{
			IdentityProvider.Setup(SecurityKey);
		}

		#endregion

		#region Methods

		#region IDisposable Members

		public void Dispose()
		{
			IdentityProvider.Reset();
		}

		#endregion

		#endregion
	}
}
