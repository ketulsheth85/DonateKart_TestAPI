using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using DonateKart_TestAPI.Models;

namespace DonateKart_TestAPI.Controllers
{
    [ApiController]
    [Route("Home")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        [Route("GetListofCampaigns")]
        public async Task<List<ResponseCampaignDetail>> GetListofCampaigns()
        {
            List<ResponseCampaignDetail> campaignlist = new List<ResponseCampaignDetail>();
            var responseBody = await GetList();
            var response = JsonConvert.DeserializeObject<List<ResponseCampaignDetail>>(responseBody);
            return response.OrderByDescending(x => x.totalAmount).Select(a => new ResponseCampaignDetail
            {
                title = a.title,
                totalAmount = a.totalAmount,
                backersCount = a.backersCount,
                endDate = a.endDate
            }).ToList();
        }
        

        [HttpGet]
        [Route("GetListofActiveCampaigns")]
        public async Task<List<ResponseCampaignDetail>> GetListofActiveCampaigns()
        {
            List<ResponseCampaignDetail> campaignlist = new List<ResponseCampaignDetail>();
            var responseBody = await GetList();
            var response = JsonConvert.DeserializeObject<List<CampaignDetail>>(responseBody);
            return response.Where(x => x.endDate >= DateTime.Now.Date && (x.created <= DateTime.Now.Date && x.created >= DateTime.Now.Date.AddDays(-30))).Select(a => new ResponseCampaignDetail
            {
                title = a.title,
                totalAmount = a.totalAmount,
                backersCount = a.backersCount,
                endDate = a.endDate
            }).ToList();
        }

        [HttpGet]
        [Route("GetListofClosedCampaigns")]
        public async Task<List<ResponseCampaignDetail>> GetListofClosedCampaigns()
        {
            List<ResponseCampaignDetail> campaignlist = new List<ResponseCampaignDetail>();
            var responseBody = await GetList();
            var response = JsonConvert.DeserializeObject<List<CampaignDetail>>(responseBody);
            return response.Where(x => x.endDate < DateTime.Now.Date || x.procuredAmount >= x.totalAmount).Select(x => new ResponseCampaignDetail
            {
                title = x.title,
                totalAmount = x.totalAmount,
                backersCount = x.backersCount,
                endDate = x.endDate
            }).ToList();
        }

        [NonAction]
        public async Task<string> GetList()
        {
            string responseString = string.Empty;
            HttpClientHandler clientHandler = new HttpClientHandler();
            using (HttpClient client = new HttpClient(clientHandler))
            {
                client.BaseAddress = new Uri("https://testapi.donatekart.com/");
                HttpRequestMessage requestMessage = new HttpRequestMessage();
                var response = await client.GetAsync("api/campaign");
                responseString = await response.Content.ReadAsStringAsync();
            }
            return responseString;
        }
    }
}
