using System;
using IdentityModel.Client;
using System.Net.Http;

public class Program
{
    public static void Main()
    {
        UseToken(GetToken1());
        UseToken(GetUserToken1());
    }


    static void UseToken(TokenResponse token)
    {
        Console.WriteLine(token.AccessToken);
        Console.WriteLine(token.Raw);
        var client = new HttpClient();
        client.SetBearerToken(token.AccessToken);
        Console.WriteLine(client.GetStringAsync("http://localhost:5000/test").Result);
    }

    static TokenResponse GetToken()
    {
        var client = new TokenClient(
             "https://localhost:44300/connect/token",
            "test",
            "secret");
        return client.RequestClientCredentialsAsync("api1").Result;
    }
    static TokenResponse GetUserToken()
    {
        var client = new TokenClient(
            "https://localhost:44300/connect/token",
            "carbon",
            "carbonsecret");

        return client.RequestResourceOwnerPasswordAsync("bob", "bob", "api1").Result;
    }

    static TokenResponse GetToken1()
    {
        var client = new TokenClient(
             "https://janitor.chinacloudsites.cn/connect/token",
            "test",
            "secret");
        return client.RequestClientCredentialsAsync("api1").Result;
    }
    static TokenResponse GetUserToken1()
    {
        var client = new TokenClient(
            "https://janitor.chinacloudsites.cn/connect/token",
            "carbon",
            "carbonsecret");

        return client.RequestResourceOwnerPasswordAsync("bob", "bob", "api1").Result;
    }
}

