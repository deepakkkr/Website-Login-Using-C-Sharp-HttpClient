using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;// Html parser, First install it from nuget.

namespace Discovery.ListApi
{
    class HackerrankLogin
    {
        public static async Task PreparePost(HttpClient client,string csrf_token)
        {
            //X-CSRF-Token: K/6bkYglSxD6GnR/3F+1xj6efwxFP0yemWHhN5DTfzEwjO7wa2WN+jOtzxsxztrYPvZkB5UQJtb8FcpyUYr8nw==            
            
            client.DefaultRequestHeaders.Add("Accept", "application / json");
            client.DefaultRequestHeaders.Add("X-CSRF-Token", csrf_token);
            client.DefaultRequestHeaders.Add("Referer","https://www.hackerrank.com/auth/login?h_l=body_middle_left_button&h_r=login");
            string cred="{ \"login\": \"email\", \"password\": \"password\", \"remember_me\": false, \"fallback\": true}";
            var response = await client.PostAsync("/rest/auth/login", new StringContent(cred, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Success");
                //If you want to verify uncomment below lines.
                //var res = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(res.ToString());
            }
            else
            {
                Console.WriteLine("Not Sccess"+response.StatusCode);
            }
        }
        
      //Ensure you have selected c# 7.1 because async keyword with Main method works with the c# 7.1 and above.  
        static async Task Main()
        {
            var baseurl = new Uri("https://www.hackerrank.com/");
            var cookieContainer = new System.Net.CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer})
            using (var client = new HttpClient(handler))
            {
                handler.AllowAutoRedirect=true;
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.132 Safari/537.36");
                client.BaseAddress = baseurl;
                var response = await client.GetAsync("/auth/login");
                if (response.IsSuccessStatusCode)
                {
                    var cnt = await response.Content.ReadAsStringAsync();
                    HtmlDocument html = new HtmlDocument();
                    html.LoadHtml(cnt.ToString());
                    HtmlNode html_node = html.GetElementbyId("csrf-token");
                    string csrf_token = html_node.GetAttributeValue("content", "Notfound");
                    if (csrf_token != "NotFound")
                    {
                        await PreparePost(client,csrf_token);
                    }
                    else
                    {
                        Console.WriteLine("CSRF token not found");
                    }
                }
                else
                {
                    Console.WriteLine(response.StatusCode);
                    Console.WriteLine("Not Success.");
                }
            }
            Console.ReadKey();
            }
    }
}
