using PITB.CMS_DB.Models;
using PITB.CMS_DB.Modules.Repository;
using PITB.CMS_DB.Modules.Repository.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PITB.CMS_DB
{
    public class Program
    {
        public static void Main()
        {
            //UnitOfWork unitOfWork = new UnitOfWork();
            //List<DbCampaign> listDbCampaign = unitOfWork.DbCampaign.Get(where: s => s.District_Id == 1,
            // orderBy: q => q.OrderBy(d => d.Campaign_Name)).ToList();
            ////List<DbCampaign> listDbCampaign = unitOfWork.DbCampaign.GetAll();
            //DbCampaign listDbCampaign2 = unitOfWork.DbCampaign.GetById(47);


            RepDbCampaign repCampaign = new RepDbCampaign(new CMSDbContext());
            List<DbCampaign> listDbCampaign3 = repCampaign.Get(where: null,
             orderBy: q => q.OrderBy(d => d.Campaign_Name)).ToList();
            ////List<DbCampaign> listDbCampaign = unitOfWork.DbCampaign.GetAll();
            //DbCampaign dbCamp2 = repCampaign.GetById(47);


            //GetCampaignShortNameById

            Console.WriteLine("Hello");
            Console.ReadLine();
        }
    }
}
