#region Namespace Sonata.IdentityModel
//	TODO: comment
#endregion

using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Security;
using System.Text;

namespace Sonata.IdentityModel
{
	public class ApplicationToken
	{
		#region Constants

		public const string ApplicationKeyKey = "appKey";
		public const string UserKey = "user";
		public const string ExpirationKey = "exp";

		#endregion

		#region Members

		private static string _symetricSecurityKey;

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
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_symetricSecurityKey));
			var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
			var header = new System.IdentityModel.Tokens.Jwt.JwtHeader(signingCredentials);
			var payload = new System.IdentityModel.Tokens.Jwt.JwtPayload { { ApplicationKeyKey, ApplicationKey }, { UserKey, UserName } };

			if (ExpirationDate.HasValue)
				payload.Add(ExpirationKey, ExpirationDate.Value);

			var jwt = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(header, payload);
			var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

			return handler.WriteToken(jwt);
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
			var token = handler.ReadJwtToken(applicationToken);

			if (!token.Payload.TryGetValue(ApplicationKeyKey, out var applicationKey))
				throw new SecurityException();

			if (!token.Payload.TryGetValue(UserKey, out var userName))
				throw new SecurityException();

			token.Payload.TryGetValue(ExpirationKey, out var expirationDateOrTimeStamp);

			if (expirationDateOrTimeStamp == null)
				return new ApplicationToken(applicationKey.ToString(), userName.ToString(), null);

			var isTimeStamp = expirationDateOrTimeStamp.ToString().All(char.IsDigit);
			if (!isTimeStamp)
			{
				if (DateTime.TryParse(expirationDateOrTimeStamp.ToString(), out var expirationDate))
					return new ApplicationToken(applicationKey.ToString(), userName.ToString(), expirationDate);

				throw new SecurityException("The specified token does not contain a valid expiration date.");
			}

			var unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			var unixTimeStampInTicks = long.Parse(expirationDateOrTimeStamp.ToString()) * TimeSpan.TicksPerSecond;

			return new ApplicationToken(
				applicationKey.ToString(), 
				userName.ToString(), 
				new DateTime(unixStart.Ticks + unixTimeStampInTicks, DateTimeKind.Utc));
		}

		internal static void Reset()
		{
			_symetricSecurityKey = null;
		}

		internal static void Setup(string symetricSecurityKey)
		{
			_symetricSecurityKey = symetricSecurityKey;
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
