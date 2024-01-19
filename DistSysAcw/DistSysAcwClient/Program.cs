using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

#region Task 10 and beyond
namespace ApiClient
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            string storedApikey = "";
            string storedUsername = "";
            string publicKey = "";
            string apiUrl = "https://localhost:44394/api/";
            Console.WriteLine("Hello. What would you like to do?");
            while (true) 
            {
                string input = Console.ReadLine();
                Console.Clear();
                if (input == "Exit") break;
                string[] inputWords = input.Split(' ');
                if (inputWords.Length < 2) Console.WriteLine("Not a valid command. Try again.");
                else
                {
                    Console.WriteLine("...please wait...");
                    switch (inputWords[0] + " " + inputWords[1])
                    {
                        case "TalkBack Hello":
                            HttpResponseMessage response = await client.GetAsync(apiUrl+"talkback/hello");
                            response.EnsureSuccessStatusCode();
                            string responseBody = await response.Content.ReadAsStringAsync();
                            Console.WriteLine(responseBody);
                            break;

                        case "TalkBack Sort":
                            string[] integers = inputWords[2].Trim('[', ']').Split(',');
                            string result = "sort";
                            for (int i = 0; i < integers.Length; i++)
                            {
                                if (i == 0)
                                {
                                    result += $"?integers={integers[i]}";
                                }
                                else
                                {
                                    result += $"&integers={integers[i]}";
                                }
                            }
                            HttpResponseMessage response2 = await client.GetAsync(apiUrl+ "talkback/" + result);
                            response2.EnsureSuccessStatusCode();
                            string response2Body = await response2.Content.ReadAsStringAsync();
                            Console.WriteLine(response2Body);
                            break;

                        case "User Get":
                            HttpResponseMessage response3 = await client.GetAsync(apiUrl + "user/new?username=" + inputWords[2]);
                            response3.EnsureSuccessStatusCode();
                            string response3Body = await response3.Content.ReadAsStringAsync();
                            Console.WriteLine(response3Body);
                            break;

                        case "User Post":
                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                            var request4 = new HttpRequestMessage(HttpMethod.Post, apiUrl + $"user/new");
                            request4.Content = new StringContent($"\"{inputWords[2]}\"", Encoding.UTF8, "application/json");
                            HttpResponseMessage response4 = await client.SendAsync(request4);

                            if (response4.IsSuccessStatusCode)
                            {
                                storedApikey = await response4.Content.ReadAsStringAsync();
                                storedApikey = storedApikey.Trim('"', '/');
                                storedUsername = inputWords[2].Trim('"', '/');
                                Console.WriteLine(storedApikey);
                            }
                            else
                            {
                                Console.WriteLine($"Error: {response4.StatusCode}");
                            }
                            break;

                        case "User Set":
                            storedUsername = inputWords[2];
                            storedApikey = inputWords[3];
                            Console.WriteLine("Stored");
                            break;
                        case "User Delete":
                            if(storedUsername!="" && storedApikey!="")
                            {
                                var request = new HttpRequestMessage(HttpMethod.Delete, apiUrl + $"user/removeuser?username={storedUsername}");
                                request.Headers.Add("ApiKey", storedApikey);
                                HttpResponseMessage response6 = await client.SendAsync(request);
                                if(response6.IsSuccessStatusCode) 
                                {
                                    Console.WriteLine("User deleted successfully");
                                }
                                else
                                {
                                    Console.WriteLine($"Error: {response6.StatusCode}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("You need to do a User Post or User Set first.");
                            }
                            break;

                        case "User Role":
                            if(storedApikey!="")
                            {
                                var userRoleRequest = new
                                {
                                    username = inputWords[2],
                                    role = inputWords[3]
                                };
                                string jsonString7 = System.Text.Json.JsonSerializer.Serialize(userRoleRequest);
                                HttpContent content7 = new StringContent(jsonString7, Encoding.UTF8, "application/json");

                                var request7 = new HttpRequestMessage(HttpMethod.Post, apiUrl + "user/changerole");
                                request7.Headers.Add("Apikey", storedApikey);
                                request7.Content = content7;

                                HttpResponseMessage response7 = await client.SendAsync(request7);
                                if(response7.IsSuccessStatusCode)
                                {
                                    Console.WriteLine("User role successfully changed.");
                                }
                                else
                                {
                                    Console.WriteLine($"Error: {response7.StatusCode}");
                                }
                            }
                            else 
                            {
                                Console.WriteLine("You need to do a User Post or User Set first.");
                            }
                            break;

                        case "Protected Hello":
                            if(storedApikey!="")
                            {
                                var request8 = new HttpRequestMessage(HttpMethod.Get, apiUrl + "protected/hello");
                                request8.Headers.Add("ApiKey", storedApikey);
                                
                                HttpResponseMessage response8 = await client.SendAsync(request8);
                                if(response8.IsSuccessStatusCode) 
                                {  
                                    string result8 = await response8.Content.ReadAsStringAsync();
                                    Console.WriteLine($"Protected Hello Response: {result8}");
                                }
                                else
                                {
                                    Console.WriteLine($"Error: {response8.StatusCode}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("You need to do a User Post or User Set first");
                            }
                            break;

                        case "Protected SHA1":
                            if(storedApikey!="")
                            {
                                var request9 = new HttpRequestMessage(HttpMethod.Get, apiUrl + "protected/sha1?message=" + inputWords[2]);
                                request9.Headers.Add("ApiKey", storedApikey);

                                HttpResponseMessage response9 = await client.SendAsync(request9);
                                if(response9.IsSuccessStatusCode) 
                                {
                                    string result9 = await response9.Content.ReadAsStringAsync();
                                    Console.WriteLine($"Protected SHA1 Response: {result9}");
                                }
                                else
                                {
                                    Console.WriteLine($"Error: {response9.StatusCode}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("You need to do a User Post or User Set first.");
                            }
                            break;

                        case "Protected SHA256":
                            if(storedApikey!="")
                            {
                                var request10 = new HttpRequestMessage(HttpMethod.Get, apiUrl + "protected/sha256?message=" + inputWords[2]);
                                request10.Headers.Add("ApiKey", storedApikey);

                                HttpResponseMessage response10 = await client.SendAsync(request10);
                                if (response10.IsSuccessStatusCode)
                                {
                                    string result10 = await response10.Content.ReadAsStringAsync();
                                    Console.WriteLine($"Protected SHA256 Response: {result10}");
                                }
                                else
                                {
                                    Console.WriteLine($"Error: {response10.StatusCode}");
                                }
                            }
                            else
                            {
                                Console.WriteLine("You need to do a User Post or User Set first.");
                            }
                            break;

                        case "Protected Get": //Protected Get PublicKey
                            if(storedApikey!="")
                            {
                                var request11 = new HttpRequestMessage(HttpMethod.Get, apiUrl + "protected/getpublickey");
                                request11.Headers.Add("ApiKey", storedApikey);

                                HttpResponseMessage response11 = await client.SendAsync(request11);
                                
                                if (response11.IsSuccessStatusCode)
                                {
                                    publicKey = await response11.Content.ReadAsStringAsync();
                                    Console.WriteLine("Got Public Key");
                                }
                                else
                                {
                                    Console.WriteLine("Couldn't Get the Public Key");
                                }
                            }
                            else
                            {
                                Console.WriteLine("You need to do a User Post or User Set first.");
                            }
                            break;

                        case "Protected Sign":
                            if(storedApikey!="")
                            {
                                if(publicKey!="")
                                {
                                    var request12 = new HttpRequestMessage(HttpMethod.Get, apiUrl + "protected/sign?message=" + inputWords[2]);
                                    request12.Headers.Add("ApiKey", storedApikey);

                                    HttpResponseMessage response12 = await client.SendAsync(request12);
                                    if(response12.IsSuccessStatusCode)
                                    {
                                        string signedMesage = await response12.Content.ReadAsStringAsync();

                                        var rsa = new RSACryptoServiceProvider();
                                        rsa.FromXmlString(publicKey);

                                        byte[] messageBytes = Encoding.ASCII.GetBytes(inputWords[2]);
                                        byte[] signatureBytes = Convert.FromBase64String(signedMesage);

                                        bool isSignatureValid = rsa.VerifyData(messageBytes, CryptoConfig.MapNameToOID("SHA1"), signatureBytes);
                                        if (isSignatureValid)
                                        {
                                            Console.WriteLine("Message was successfully signed");
                                        }
                                        else
                                        {
                                            Console.WriteLine("Message was not successfully signed");
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Error: {response12.StatusCode}");
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Client doesn't yet have the public key");
                                }
                            }
                            else
                            {
                                Console.WriteLine("You need to do a User Post or User Set first.");
                            }
                            break;

                        case "Protected AddFifty":
                            
                            break;

                        default:
                            Console.WriteLine("Not a valid command. Try again.");
                            break;
                    }
                }
                Console.WriteLine("What would you like to do next?");
            }
        }
    }
}
#endregion