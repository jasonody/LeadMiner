using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace LeadMiner.Api.Controllers
{
	[RoutePrefix("login")]
	[EnableCors("*", "*", "*")]
    public class LoginController : ApiController
    {
		[Route("facebook")]
		[HttpPost]
		public async Task<HttpResponseMessage> LoginWithFacebook([FromUri] string code)
		{
			Uri eatTargetUri = 
				new Uri("https://graph.facebook.com/oauth/access_token?"
				+ "&client_id="	+ ConfigurationManager.AppSettings["FacebookAppId"]
				+ "&redirect_uri=http://localhost:3000/%23/facebook/"
				+ "&client_secret=" + ConfigurationManager.AppSettings["FacebookAppSecret"] 
				+ "&code=" + code);

			string token = "";

			using (var client = new HttpClient())
			{
				var response = await client.GetAsync(eatTargetUri);
				if (response.IsSuccessStatusCode)
				{
					token = await response.Content.ReadAsStringAsync();
					var parts = token.Split('&');

					//TODO Verify user with Facebook ID

					return Request.CreateResponse(HttpStatusCode.OK, new 
					{
						Token = parts[0].Split('=')[1],
						Expires = parts[1].Split('=')[1]
					});
				}
			}

			return Request.CreateResponse(HttpStatusCode.InternalServerError, new { Error = "Something went wrong" });
		}
    }
}
