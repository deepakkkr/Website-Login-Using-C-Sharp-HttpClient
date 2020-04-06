using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace HttpClientLogin
{
    class CodechefLogin
    {

        public static async Task PreparePost(HttpClient client,HtmlDocument html)
        {
                 

            client.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            client.DefaultRequestHeaders.Add("Referer", "https://www.codechef.com/");

            //Process form data.
            var html_node = html.DocumentNode.Descendants("input").FirstOrDefault(n => n.Attributes["name"].Value == "form_build_id");
            var form_data = new FormUrlEncodedContent(new[] {
                                 new KeyValuePair<string,string>("name","Your username"),
                                 new KeyValuePair<string,string>("pass","Your password"),
                                 new KeyValuePair<string,string>("csrfToken",html.GetElementbyId("edit-csrfToken").GetAttributeValue("value","Notfound")),
                                 new KeyValuePair<string,string>( "form_build_id",html_node.GetAttributeValue("value","Notfound") ),
                                 new KeyValuePair<string,string>("form_id",html.GetElementbyId("edit-new-login-form").GetAttributeValue("value","Notfound")),
                                 new KeyValuePair<string,string>("op",html.GetElementbyId("edit-submit").GetAttributeValue("value","Notfound"))
            });

            var response = await client.PostAsync("/", form_data);
            var stringContent = await response.Content.ReadAsStringAsync();
            var html_res = new HtmlDocument();
            html_res.LoadHtml(stringContent.ToString());
            //If the program logged in successfully then the one of anchor tag in html response will contain "/logout" 
            HtmlNode html_node_log =html_res.DocumentNode.SelectNodes("//a[@href]").FirstOrDefault(n => n.Attributes["href"].Value == "/logout");

            if (response.IsSuccessStatusCode && html_node_log != null && html_node_log.GetAttributeValue("href", "Notfound") != "Notfound")
            {
                Console.WriteLine("Success");
                Console.WriteLine("Press a key to logout..................");
                Console.ReadKey();
                await CodechefLogin.logout(client);
                Console.WriteLine("Press a key to exit");
                Console.ReadKey();
            }

            else
            {
                Console.WriteLine("Not Sccess");
            }
        }


        static async Task logout(HttpClient client)
        {
            var response = await client.GetAsync("/logout");

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("You have been logout.");
            }
        }

        static async Task Main()
        {
            var baseurl = new Uri("https://www.codechef.com/");
            var cookieContainer = new System.Net.CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler))
            {
                //handler.AllowAutoRedirect = true;
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.132 Safari/537.36");
                client.BaseAddress = baseurl;
                var response = await client.GetAsync("/");
                if (response.IsSuccessStatusCode)
                {
                    var cnt = await response.Content.ReadAsStringAsync();
                    HtmlDocument html = new HtmlDocument();
                    html.LoadHtml(cnt.ToString());

                    if (html.DocumentNode!=null)
                    {
                       await PreparePost(client,html);
                    }
                    else
                    {
                        Console.WriteLine("Html content is empty.");
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
