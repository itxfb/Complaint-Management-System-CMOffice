using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM_API.Models.API
{
    public class LwmcModel
    {
        public class Api
        {
            public class Request
            {
                public class GoogleNotification
                {
                    public string to { get; set; }

                    public Data data { get; set; }

                    public GoogleNotification()
                    {
                        this.data = new Data();
                    }

                    public class Data
                    {
                        //public int complaintId { get; set; }

                        //public string description { get; set; }

                        public string message { get; set; }

                        public string title { get; set; }

                        public string detail { get; set; }
                    } 
                    
                
                 }
            }
            public class Response
            {

            }
        }

    }
}