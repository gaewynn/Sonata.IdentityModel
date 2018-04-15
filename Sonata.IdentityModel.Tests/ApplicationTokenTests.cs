using System;
using Sonata.IdentityModel.Tests.Fixtures;
using Xunit;

namespace Sonata.IdentityModel.Tests
{
	[CollectionDefinition("ApplicationToken_Collection")]
	public class ApplicationTokenCollection : ICollectionFixture<IdentityProviderFixture>
	{
		// This class has no code, and is never created. Its purpose is simply
		// to be the place to apply [CollectionDefinition] and all the
		// ICollectionFixture<> interfaces.
	}

	[Collection("ApplicationToken_Collection")]
	public class ApplicationTokenTests
	{
		[Fact]
		public void GenerateTokenWithoutExpirationDate()
		{
			const string expected = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhcHBLZXkiOiJTb25hdGEiLCJ1c2VyIjoiZ2Fld3lubiJ9.PZS5CvHf1ArU-7iQakVBokeEfUh8DKqMv08nnrXgBPM";
			var applicationToken = new ApplicationToken("Sonata", "gaewynn");

			Assert.Equal(expected, applicationToken.WriteToken());
		}

		[Fact]
		public void GenerateTokenWithExpirationDate()
		{
			const string expected = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhcHBLZXkiOiJTb25hdGEiLCJ1c2VyIjoiZ2Fld3lubiIsImV4cCI6IjE5ODMtMDctMTNUMDA6MDA6MDAifQ.rLdO_8rujN-d61KLWTjWzosb3U8aQ2EXCPkhF3VCy9M";
			var applicationToken = new ApplicationToken("Sonata", "gaewynn", new DateTime(1983, 7, 13));

			Assert.Equal(expected, applicationToken.WriteToken());
		}

		[Fact]
		public void TokenContainsAllInformationWithoutExpirationDate()
		{
			var applicationToken = new ApplicationToken("Sonata", "gaewynn");
			var serializedToken = applicationToken.WriteToken();
			var deserializedToken = ApplicationToken.ReadToken(serializedToken);

			Assert.Equal("Sonata", deserializedToken.ApplicationKey);
			Assert.Equal("gaewynn", deserializedToken.UserName);
			Assert.Null(deserializedToken.ExpirationDate);
		}

		[Fact]
		public void TokenContainsAllInformationWithExpirationDate()
		{
			var applicationToken = new ApplicationToken("Sonata", "gaewynn", new DateTime(1983, 7, 13));
			var serializedToken = applicationToken.WriteToken();
			var deserializedToken = ApplicationToken.ReadToken(serializedToken);

			Assert.Equal("Sonata", deserializedToken.ApplicationKey);
			Assert.Equal("gaewynn", deserializedToken.UserName);
			Assert.Equal(new DateTime(1983, 7, 13), deserializedToken.ExpirationDate);
		}
	}
}