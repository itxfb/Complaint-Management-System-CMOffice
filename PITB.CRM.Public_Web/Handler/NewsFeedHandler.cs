

using System;
using PITB.CRM.Public_Web.Models.ViewModels;

namespace PITB.CRM.Public_Web.Handler
{
    public class NewsFeedHandler
    {
        public static VmNewsFeed GetNewsFeed()
        {
            VmNewsFeed vm = new VmNewsFeed();
            vm.ListComplaints.Add(new ComplaintData()
            {
                ComplaintId = 170055,
                Category = "Environmental pollution and contamination by Industries",
            //    Status = "Pending (Fresh)",
                Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit.olorem quae quo recusandae similique",
                District = "Lahore",
                Town = "Model Town",
                Location = new Location() { Latitude = 31.257155, Longitude = 74.22268 },
                
              CampaignId = 49,
                SocialSharedData=new SocialShareData()
                {
                    UserId = "100000918188328",
                    FirstName = "Jhon",
                    LastName = "Atif",
                    DateTime = DateTime.Now,
                    Provider = "Facebook",
                    
                } ,
              
                

            }

                );
            return vm;
        }
    }
}
