using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;

public class decryptService
{

    IConfiguration appsettings = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

    public string getJwtToken()
    {
        var unm="pravinsingh";
        var uid="001"+Guid.NewGuid().ToString();

        //GENERATE TOKEN HERE IF USER IS VALID
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, unm),
            new Claim(ClaimTypes.NameIdentifier,uid)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appsettings["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
        var tokenDescriptor = new JwtSecurityToken(issuer: appsettings["Jwt:Issuer"], audience: appsettings["Jwt:Audience"], claims : claims,
        expires: DateTime.Now.AddMinutes(Int16.Parse(appsettings["Jwt:ExpiryDuration"])), signingCredentials: credentials);
        var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        return token.ToString();
        // IDictionary<string, Object> retData = new Dictionary<string, Object>();
        // retData.Add("1","One"); //adding a key/value using the Add() method
        // retData.Add("2","Two");
        // retData.Add("3","Three");    
        // resData.eventID=reqData.eventID;
        // resData.rStatus=0;
        // resData.rData= retData;
    }


    public string getHash(string stringToCompute)
    {
        SHA256 mySHA256 = SHA256.Create();
        // convert string to byte array
        byte[] strToComputeBytes=Encoding.UTF8.GetBytes(stringToCompute);
        byte[] byteHash = mySHA256.ComputeHash(strToComputeBytes);
        return BitConverter.ToString(byteHash).Replace("-","");
    }

    public string AESDecrypt(string base64Key, string base64Ciphertext)
    {
        // convert from base64 to raw bytes spans
        var encryptedData = Convert.FromBase64String(base64Ciphertext).AsSpan();
        var key = Convert.FromBase64String(base64Key).AsSpan();

        var tagSizeBytes = 16; // 128 bit encryption / 8 bit = 16 bytes
        var ivSizeBytes = 12; // 12 bytes iv
    
        // ciphertext size is whole data - iv - tag
        var cipherSize = encryptedData.Length - tagSizeBytes - ivSizeBytes;

        // extract iv (nonce) 12 bytes prefix
        var iv = encryptedData.Slice(0, ivSizeBytes);
    
        // followed by the real ciphertext
        var cipherBytes = encryptedData.Slice(ivSizeBytes, cipherSize);

        // followed by the tag (trailer)
        var tagStart = ivSizeBytes + cipherSize;
        var tag = encryptedData.Slice(tagStart);

        // now that we have all the parts, the decryption
        Span<byte> plainBytes = cipherSize < 1024
            ? stackalloc byte[cipherSize]
            : new byte[cipherSize];

        //var ae= new AesCng();    
        var aes = new AesGcm(key);
        

        aes.Decrypt(iv, cipherBytes, tag, plainBytes);
        return Encoding.UTF8.GetString(plainBytes);
    }



    // this function is called only once copy the publoc and private key from the console to be shared
    public  void GeneratePrivatePublicKeyPair() 
    {
        var name = "test";
        //var privateKeyXmlFile = name + "_priv.xml";
        //var publicKeyXmlFile = name + "_pub.xml";
        //var publicKeyFile = name + ".pub";

        using var provider = new RSACryptoServiceProvider(1024);
        Console.Write(provider.ToXmlString(true)); // private key
        Console.Write(provider.ToXmlString(false)); // public key
        //var x = provider.ImportRSAPrivateKey();
        //provider.ToString(true)
        //File.WriteAllText(privateKeyXmlFile, provider.ToXmlString(true));
        //File.WriteAllText(publicKeyXmlFile, provider.ToXmlString(false));
        //using var publicKeyWriter = File.CreateText(publicKeyFile);
        //ExportPublicKey(provider, publicKeyWriter);
        var x=1;
    }

    public void testCrypto()
    {
      //lets take a new CSP with a new 2048 bit rsa key pair
      var csp = new RSACryptoServiceProvider(2048);

      //how to get the private key
      var privKey = csp.ExportParameters(true);

      //and the public key ...
      var pubKey = csp.ExportParameters(false);

      //converting the public key into a string representation
      string pubKeyString;
      {
        //we need some buffer
        var sw = new System.IO.StringWriter();
        //we need a serializer
        var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
        //serialize the key into the stream
        xs.Serialize(sw, pubKey);
        //get the string from the stream
        pubKeyString = sw.ToString();
      }
      //csp.ExportRSAPublicKey()
   
      //converting it back
      {
        //get a stream from the string
        var sr = new System.IO.StringReader(pubKeyString);
        //we need a deserializer
        var xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
        //get the object back from the stream
        pubKey = (RSAParameters)xs.Deserialize(sr);
      }

      //conversion for the private key is no black magic either ... omitted

      //we have a public key ... let's get a new csp and load that key
      csp = new RSACryptoServiceProvider();
      csp.ImportParameters(pubKey);

      //we need some data to encrypt
      var plainTextData = "foobar";

      //for encryption, always handle bytes...
      var bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes(plainTextData);

      //apply pkcs#1.5 padding and encrypt our data 
      var bytesCypherText = csp.Encrypt(bytesPlainTextData, false);

      //we might want a string representation of our cypher text... base64 will do
      var cypherText = Convert.ToBase64String(bytesCypherText);


      /*
       * some transmission / storage / retrieval
       * 
       * and we want to decrypt our cypherText
       */

      //first, get our bytes back from the base64 string ...
      bytesCypherText = Convert.FromBase64String(cypherText);

      //we want to decrypt, therefore we need a csp and load our private key
      csp = new RSACryptoServiceProvider();
      csp.ImportParameters(privKey);

      //decrypt and strip pkcs#1.5 padding
      bytesPlainTextData = csp.Decrypt(bytesCypherText, false);

      //get our original plainText back...
      plainTextData = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);
    }


    private static void DecryptPrivate(string dataToDecrypt)
    {
        var name = "test";
        var encryptedBase64 = @"Rzabx5380rkx2+KKB+HaJP2dOXDcOC7SkYOy4HN8+Nb9HmjqeZfGQlf+ZUa6uAfAJ3oAB2iIlHlnx+iXK3XDIX3izjoW1eeiNmdOWieNCu6YXqW4denUVEv0Z4EpAmEYgVImnEzoMdmPDEcl9UHgdWUmS4Bnq6T8Yqh3UZ/4NOc=";
        var encrypted = Convert.FromBase64String(encryptedBase64);
        using var privateKey = new RSACryptoServiceProvider();
        privateKey.FromXmlString(File.ReadAllText(name + "_priv.xml"));
        var decryptedBytes = privateKey.Decrypt(encrypted, false);
        var dectryptedText = Encoding.UTF8.GetString(decryptedBytes);
    }

    private static void EncryptPrivate(string dataToDecrypt)
    {
        var name = "test";
        var encryptedBase64 = @"Rzabx5380rkx2+KKB+HaJP2dOXDcOC7SkYOy4HN8+Nb9HmjqeZfGQlf+ZUa6uAfAJ3oAB2iIlHlnx+iXK3XDIX3izjoW1eeiNmdOWieNCu6YXqW4denUVEv0Z4EpAmEYgVImnEzoMdmPDEcl9UHgdWUmS4Bnq6T8Yqh3UZ/4NOc=";
        var encrypted = Convert.FromBase64String(encryptedBase64);
        using var privateKey = new RSACryptoServiceProvider();
        privateKey.FromXmlString(File.ReadAllText(name + "_priv.xml"));
        var decryptedBytes = privateKey.Decrypt(encrypted, false);
        var dectryptedText = Encoding.UTF8.GetString(decryptedBytes);

        
    }
}