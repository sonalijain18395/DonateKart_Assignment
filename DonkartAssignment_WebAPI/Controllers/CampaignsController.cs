using AutoMapper;
using DonkartAssignment_WebAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace DonkartAssignment_WebAPI.Controllers
{
  public class CampaignsController : ApiController
  {
    [HttpGet]
    public string GetResponse()
    {
      using (var client = new HttpClient())
      {
        client.BaseAddress = new Uri("https://testapi.donatekart.com/");
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        HttpResponseMessage response = client.GetAsync("api/campaign").Result;
        if (response.IsSuccessStatusCode)
        {
          var result = response.Content.ReadAsStringAsync().Result;
          return result;
        }
        else
          return null;
      }
    }

    //api/Campaigns/GetCampaigns
    public string GetCampaigns()
    {
      var responseResult = GetResponse();
      if (responseResult != null)
      {
        List<CampaignModel> campaignDTO = JsonConvert.DeserializeObject<List<CampaignModel>>(responseResult);
        var selectSpecificColumn = campaignDTO.Select(x => new { Title = x.title, TotalAmount = x.totalAmount, BackersCount = x.backersCount, EndDate = x.endDate })
                           .OrderByDescending(x => x.TotalAmount)
                           .ToList();
        return JsonConvert.SerializeObject(selectSpecificColumn);
      }
      else
        return "Something Went Wrong.";
    }

    //api/Campaigns/GetActiveCampaigns
    public string GetActiveCampaigns()
    {
      var responseResult = GetResponse();
      if (responseResult != null)
      {
        List<CampaignModel> campaignDTO = JsonConvert.DeserializeObject<List<CampaignModel>>(responseResult)
                                                     .Where(x => x.endDate >= DateTime.Today && x.created > DateTime.Now.AddDays(-30))
                                                     .ToList();
        return JsonConvert.SerializeObject(campaignDTO);
      }
      else
        return "Something Went Wrong.";
    }

    //api/Campaigns/GetClosedCampaigns
    public string GetClosedCampaigns()
    {
      var responseResult = GetResponse();
      if (responseResult != null)
      {
        List<CampaignModel> campaignDTO = JsonConvert.DeserializeObject<List<CampaignModel>>(responseResult)
                                                     .Where(x => x.endDate < DateTime.Today || x.procuredAmount >= x.totalAmount)
                                                     .ToList();
        return JsonConvert.SerializeObject(campaignDTO);
      }
      else
        return "Something Went Wrong.";
    }
  }
}
