using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;
using Microsoft.SqlServer.Server;

namespace PITB.CMS_DB.Models
{

    [Table("PITB.User_Wise_Devices")]
    public partial class DbUserWiseDevices
    {
        public int Id { get; set; }

        public int? User_Id { get; set; }

        public int? Platform_Id { get; set; }

        public string Tag_Id { get; set; }

        public string Device_Id { get; set; }

        public bool? Is_Active { get; set; }

        [ForeignKey("User_Id")]
        public virtual DbUsers DbUser { get; set; }
    }
}
