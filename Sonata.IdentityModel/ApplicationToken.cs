#region Namespace Sonata.IdentityModel
//	TODO: comment
#endregion

using System;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Xml.Serialization;
using Microsoft.IdentityModel.Tokens;
using Sonata.Core.Extensions;

namespace Sonata.IdentityModel
{
	public class ApplicationToken
	{
		#region Constants

		public const string ApplicationKeyKey = "appKey";
		public const string UserKey = "user";
		public const string ExpirationKey = "exp";
		private const string PrivateKeyBase64Encoded = "PD94bWwgdmVyc2lvbj0iMS4wIiBlbmNvZGluZz0idXRmLTE2Ij8+DQo8UlNBUGFyYW1ldGVycyB4bWxuczp4c2k9Imh0dHA6Ly93d3cudzMub3JnLzIwMDEvWE1MU2NoZW1hLWluc3RhbmNlIiB4bWxuczp4c2Q9Imh0dHA6Ly93d3cudzMub3JnLzIwMDEvWE1MU2NoZW1hIj4NCiAgPEV4cG9uZW50PkFRQUI8L0V4cG9uZW50Pg0KICA8TW9kdWx1cz52K0NDYTJEeXhvckFYVFFBS1MzRFdDNTV3ZTRUUkRyZlpqUlhaaC9yem16dEU1WjJ3dmVEb1NMbDRJaUhRL1Q1RytLV2J6Wlp3TGFlWGxIU2dNdlo0R0diRlQxejNSTWlBVzlyR3JjQThPVWw2ZmdpQStrKzVUUml4THROVW9ESjQvcDJlbU9ETXJOMUJQRzk1WmkwS25UejJZUUdjTHJZd01HV3dzT2xVZEZyTjdRRHR0aUtGTVhzT3l1QXdpejkreSsxZG5mWVhTTjl4d0UzelFTdlY4aS93bS9GOStiYUx5amhiaTRNamNpUlA4TFIydjczeVpscnlKVkdqL0JuMXpQcnRuR2srd25jOFBibFl4bmhuMEhkVktrMHBWNmJtWXl3QkYwUzJBbnlvUzBpeVIzZ3ljcFAvRFhHcWYyaVRkMit5K3JML2tFRTVPMGd0OHpuNnc9PTwvTW9kdWx1cz4NCiAgPFA+d0pHWWFIQ3VuYmpxNE1reHlnaEtmTWRQbUdlSjFZaC9oMFhBb2dzQ00wZUdKTXhRdjcxUEJ5ejNxWkFtTjRGWU5XQ2xwRjYrU01aSkk1cnlGR2lUendrcDJodHRteUZLbWdIREFKQzhPZkI0MTJ6Y0FxZDN4bFFENGRZaFNncTNINzJZR0I4MGpmZ3I3ZFFvYytFMzlzTDJ1TExOY05CT3VNUUQ0UzVTWDNVPTwvUD4NCiAgPFE+L3hTVk0zSWIvTkpncTBJL0NJc2QxRTY4VkJwZndXYitKM2FPeHNTenY4RXJmSTUxNXNsbkRzSTY3L1hQRzNwL3ovcTdFSHVGMTRieklRcUJ2cEhRMjhPVW1VdS9hUW0zaGRRUlZlb0ZBREQra1luamtIWmZTQURtaWM3Zm04czFFN3VEN3hyYm54NXk2N2QrbFp6a3lBMURtSFErS0ZBTVJGYlZXZnhkbmQ4PTwvUT4NCiAgPERQPkQrYjJFd25iSWFNM29rQjdJQmYzTHI2Mnovc2M0c2xXM3JEZjY0SEZPTDdiVVFhZVZIY3BvNG9QR2EyUnVsdXdFUmhlRldqTmhlejU5VHJDYXZVSDM3ejkrOGp3RURON21hUXpta1JaaHQ5WTIxRmVDY29TbGdveUYrQTY5NFpUQVFnREcxcllXREQ2VDdXSmNhaFRtc2tXM3NIRlBWKzJZSU92WGxVWThGRT08L0RQPg0KICA8RFE+cWs4bTB5T1VGamlzdkl2TnBEbW1YL04yQUwxZlE3VXRhbEhaUUZOakpGdW9XbENQM2RpSkM2eVpSMUFheU9FQVoyK2o2SHhndjZkbHU2RFZRZFQvMjB0NWN1VEZWeTlhcm1zS2JBK0U2VHpOSzRVaVFIa1lZdTNlL3hIMThJSTczUUh0TS83OXRmN1JnaHFRMVBuZE1zalFZZ0R5NDEyVzR5WUs2enI1a29jPTwvRFE+DQogIDxJbnZlcnNlUT5aS29iY2dXeGgweGlXSEo4cXNSZjA2ak1BWGljcGZyVmhxbE1hYWtwNW5jRm5uMzJrMGZXTkhtRllUbWxvN3gveUJvcHJOTHpkcFV1WXhpcjNEenRCK0VLUHpxampxaGYvZTg5MnI3MmdIUUJVRDFwVW1NaFp1TEM1b1lEcnpNdHFneDVrMTRTMi8rMFNMbWZWNXpJYitLWFZNLzJlVGVEYzNnY3h2UWo3THM9PC9JbnZlcnNlUT4NCiAgPEQ+RHRTaUQvMm0xaXk4VUpmbjJVYjF2ZGVkeHAwSnRURGR1bzhhSUVVZ1cweTFDRm9OWk9WdnNhRTZabC8zSUl3TFF4b0E2YWlvU0VwMU5ESWIxS09zKy9oZlhjVnBzckU3WkNRQ0FjY2FWZ1JJS1d6cDZDWjdtNGl3aDZjNGcwTjhGMWh4RnVXaEt1YVBDV3BMOUxhWVdsMit5Vlc0ajdjSmNPQ2xyL2tpU3BBRUlWUGpxNUxHTEhpZWZhSHV1Nk9zLzJzRUtjN2pCTUFYb0NwOUpyd1N0d1JBNzk4MUZIRWlBaFdkbWpma05BRUZOd3BVVm8yYWdUekwvUjJsU3U1SmxzamNtemZoUENNVS9VZHZCMHR1NnJ5ZXdQR0JjREhNZS8wVXp0QVhsSDNMSENReVhweS9BbjYrQVZZdkRmaXNiTFlZai9GR2c3czNaK21neVFidXBRPT08L0Q+DQo8L1JTQVBhcmFtZXRlcnM+";

		#endregion

		#region Members

		private static string _privateKeyBase64Encoded = PrivateKeyBase64Encoded;
		private static string _encryptionKey;

		#endregion

		#region Properties

		public string ApplicationKey { get; set; }

		public string UserName { get; set; }

		public DateTime? ExpirationDate { get; set; }

		#endregion

		#region Constructors

		public ApplicationToken(string applicationKey, string userName)
			: this(applicationKey, userName, null)
		{
		}

		public ApplicationToken(string applicationKey, string userName, DateTime? expirationDate)
		{
			ApplicationKey = applicationKey;
			UserName = userName;
			ExpirationDate = expirationDate;
		}

		#endregion

		#region Methods

		public string WriteToken()
		{
			var securityKey = new RsaSecurityKey(BuildProvider());
			var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256Signature);
			var header = new System.IdentityModel.Tokens.Jwt.JwtHeader(signingCredentials);
			var payload = new System.IdentityModel.Tokens.Jwt.JwtPayload {{ ApplicationKeyKey, ApplicationKey}, { UserKey, UserName}};

			if (ExpirationDate.HasValue)
				payload.Add(ExpirationKey, ExpirationDate.Value);

			var jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(header, payload);
			var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

			return handler.WriteToken(jwt).Encrypt(_encryptionKey);
		}

		public static ApplicationToken ValidateToken(string applicationToken)
		{
			try
			{
				var token = ReadToken(applicationToken);
				if (String.IsNullOrWhiteSpace(token.ApplicationKey))
					return null;

				return String.IsNullOrWhiteSpace(token.UserName) ? null : token;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static ApplicationToken ReadToken(string applicationToken)
		{
			var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
			var token = handler.ReadJwtToken(applicationToken.Decrypt(_encryptionKey));

			if (!token.Payload.TryGetValue(ApplicationKeyKey, out var applicationKey))
				throw new SecurityException();

			if (!token.Payload.TryGetValue(UserKey, out var userName))
				throw new SecurityException();

			token.Payload.TryGetValue(ExpirationKey, out var expirationDate);

			return new ApplicationToken(applicationKey.ToString(), userName.ToString(), (DateTime?)expirationDate);
		}

		internal static void Reset(bool useDefaultKey = false)
		{
			_privateKeyBase64Encoded = useDefaultKey ? PrivateKeyBase64Encoded : null;
			_encryptionKey = null;
		}

		internal static void Setup(string rsaPrivateKey, string encryptionKey)
		{
			_privateKeyBase64Encoded = rsaPrivateKey ?? PrivateKeyBase64Encoded;
			_encryptionKey = encryptionKey;
		}

		private static RSACryptoServiceProvider BuildProvider()
		{
			var sr = new StringReader(_privateKeyBase64Encoded.FromBase64());
			var xs = new XmlSerializer(typeof(RSAParameters));
			var rsaParamters = (RSAParameters)xs.Deserialize(sr);

			var rsaCryptoServiceProvider = new RSACryptoServiceProvider();
			rsaCryptoServiceProvider.ImportParameters(rsaParamters);

			return rsaCryptoServiceProvider;
		}

		#endregion
	}
}


/*
	Utilisation d'un algorithme de cryptage asymétrique

	//	Création d'un provider avec une clé de 2048 bits.
	var csp = new RSACryptoServiceProvider(2048);

	//	Obtention de la clé privée
	var privKey = csp.ExportParameters(true);

	//	Obtention de la clé publique
	var pubKey = csp.ExportParameters(false);

	//	Conversion de la clé publique en string
	string pubKeyString;
	{
		var sw = new System.IO.StringWriter();
		var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
		xs.Serialize(sw, pubKey);
		pubKeyString = sw.ToString();
	}

	//	Conversion de la clé publique dans l'autre sens
	{
		var sr = new System.IO.StringReader(pubKeyString);
		var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
		pubKey = (RSAParameters)xs.Deserialize(sr);
	}

	//	======================================================
	
	//	Si on a déjà une clé publique --> Création d'un provider et on importe la clé
	csp = new RSACryptoServiceProvider();
	csp.ImportParameters(pubKey);

	//	Les données à encrypter
	var plainTextData = "Il n'y a rien de plus synonyme de succès que d'être libre à 15h un mardi...J'y arriverai !";
	var bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(plainTextData);

	//	Encryptage en utilisant un padding pkcs#1.5
	var bytesCypherText = csp.Encrypt(bytesPlainTextData, false);

	//	Pour obtenir une représentation en base64 de notre texte encrypté
	var cypherText = Convert.ToBase64String(bytesCypherText);

	//	======================================================

	//	Obtention des données à partir d'une base64
	bytesCypherText = Convert.FromBase64String(cypherText);

	//	Pour décrypter le texte
	csp = new RSACryptoServiceProvider();
	csp.ImportParameters(privKey);
			
	bytesPlainTextData = csp.Decrypt(bytesCypherText, false);
	plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);
	 
	 */
