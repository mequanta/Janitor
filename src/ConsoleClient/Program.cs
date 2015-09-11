using System;
using IdentityModel.Client;
using System.Net.Http;
using Newtonsoft.Json.Linq;

public class Program
{
    public static void Main()
    {
        string local = "https://localhost:44333/connect/token";
		string remote = "https://janitor.chinacloudsites.cn/connect/token";
		string apiUrl = "http://localhost:5000/test";
	   // DoCall(local, apiUrl);
        DoCall(remote, apiUrl);
    }
	
	static void DoCall(string tokenUrl, string apiUrl)
	{
	    var token1 = GetToken(tokenUrl, "test", "secret", "api1");
	    Console.WriteLine(CallApi(token1, apiUrl));
        token1 = GetToken(tokenUrl, "test1", "secret1", "api1", "bob", "bob");
	    Console.WriteLine(CallApi(token1, apiUrl));	
	}
    static string CallApi(TokenResponse token, string endpoint)
    {
        Console.WriteLine(token.AccessToken);
        var client = new HttpClient();
        client.SetBearerToken(token.AccessToken);
        return client.GetStringAsync(endpoint).Result;
    }

    static TokenResponse GetToken(string tokenUrl, string clientId, string secret, string scope, string username = null, string password = null)
    {
        var client = new TokenClient(tokenUrl, clientId, secret);
       if (string.IsNullOrWhiteSpace(username)||string.IsNullOrWhiteSpace(password))
            return client.RequestClientCredentialsAsync(scope).Result;
        else
            return client.RequestResourceOwnerPasswordAsync(username, password, scope).Result;
    }
}

