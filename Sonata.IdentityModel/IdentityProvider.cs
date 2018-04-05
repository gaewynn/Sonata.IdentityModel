#region Namespace Sonata.IdentityModel
//	TODO: comment
#endregion

using System;
using Sonata.Core.Extensions;

namespace Sonata.IdentityModel
{
	public class IdentityProvider
	{
		public static void Setup(string rsaPrivateKey, string encryptionKey)
		{
			try
			{
				if (encryptionKey != null)
					String.Empty.Encrypt(encryptionKey);
			}
			catch (ArgumentException ex)
			{
				throw new ArgumentException("La clé d'encrpytion est invalide. Maximum 32 caractères.", ex);
			}

			ApplicationToken.Setup(rsaPrivateKey, encryptionKey);
		}

		public static void Setup(string encryptionKey)
		{
			Setup(null, encryptionKey);
		}

		public static void Setup()
		{
			Setup(null);
		}

		public static void Reset(bool useDefaultKey = false)
		{
			ApplicationToken.Reset(useDefaultKey);
		}
	}
}