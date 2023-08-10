using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Common.Handler.Configuration;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models.Custom;

namespace PITB.CMS_Handlers.Complaint
{
    public class StakeholderListingLogic
    {

        public static string GetListingQuery(ListingParamsModelBase listingModelBase)
        {
            string HierarchyCheck = "";
            string UserDesignationHierarchyCheck;
            string FinalSQL = "";
            string MulticolumnSearchQuery;

            MulticolumnSearchQuery = listingModelBase.WhereOfMultiSearch + listingModelBase.WhereLogic;
            /*if ((Config.StakeholderComplaintListingType) listingModelBase.ListingType ==
                Config.StakeholderComplaintListingType.UptilMyHierarchy)
            {
                listingModelBase.UserHierarchyId++;
            }*/
            switch ((Config.Hierarchy) listingModelBase.UserHierarchyId)
            {
                case Config.Hierarchy.Province:
                    HierarchyCheck =
                        string.Format(" and complaints.Province_Id in ({0})", listingModelBase.ProvinceId);
                    break;

                case Config.Hierarchy.Division:
                    HierarchyCheck =
                        string.Format(" and complaints.Division_Id in ({0})", listingModelBase.DivisionId);
                        //" and EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.DivisionId+@"',',') X WHERE X.Item=complaints.Division_Id)";
                    break;

                case Config.Hierarchy.District:
                    HierarchyCheck =
                        string.Format(" and complaints.District_Id in ({0})", listingModelBase.DistrictId);
                    //" and EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.DistrictId+@"',',') X WHERE X.Item=complaints.District_Id)";
                    break;

                case Config.Hierarchy.Tehsil:
                    HierarchyCheck = string.Format(" and complaints.Tehsil_Id in ({0})", listingModelBase.Tehsil);
                    //" and EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.Tehsil + @"',',') X WHERE X.Item=complaints.Tehsil_Id)";
                    break;

                case Config.Hierarchy.UnionCouncil:
                     HierarchyCheck = string.Format(" and complaints.UnionCouncil_Id in ({0})", listingModelBase.UcId);
                    //" and EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.UcId + @"',',') X WHERE X.Item=complaints.UnionCouncil_Id)";
                    break;

                case Config.Hierarchy.Ward:
                    HierarchyCheck = string.Format(" and complaints.Ward_Id in ({0})", listingModelBase.WardId);
                    //" and EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.WardId + @"',',') X WHERE X.Item=complaints.Ward_Id)";
                    break;
            }

            DateTime currDate = DateTime.Now;
            string ComplaintTypeSelection = "";
            string ComplaintTypeHierarchyCheck = "";
            string ListingTypeCheck = "";
            string CheckIfLowerHierarchyAndExistInSrcId;
            string usercategoryCheck = "";
            string regionHierarchyCheck = HierarchyCheck;

            if ((Config.ComplaintType) listingModelBase.ComplaintType == Config.ComplaintType.Complaint) // Complaint
            {
                ComplaintTypeSelection =
                    ",Complaint_Computed_Hierarchy_Id, Complaint_Computed_User_Hierarchy_Id, UserCategoryId1, UserCategoryId2, complaints.Complaint_Computed_Status as Complaint_Computed_Status, complaints.Complaint_Computed_Hierarchy  as Complaint_Computed_Hierarchy,  complaints.FollowupCount ";

                if ((Config.StakeholderComplaintListingType) listingModelBase.ListingType ==
                    Config.StakeholderComplaintListingType.AssignedToMe) //  Assigned To Me
                {
                    if (listingModelBase.UserDesignationHierarchyId != null) //AND @UserDesignationHierarchyId<>0)
                    {
                        ComplaintTypeSelection = ComplaintTypeSelection +
                                                 ", complaints.Computed_Remaining_Time_To_Escalate as Computed_Remaining_Time_To_Escalate";
                        HierarchyCheck = HierarchyCheck +
                                         " and complaints.Complaint_Computed_User_Hierarchy_Id >= " + listingModelBase.UserDesignationHierarchyId+"";
                    }
                    if (!listingModelBase.IgnoreComputedHierarchyCheck)
                    {
                        ListingTypeCheck =
                            " AND complaints.Complaint_Computed_Hierarchy_Id <= " + listingModelBase.UserHierarchyId;
                    }

                    ListingTypeCheck = ListingTypeCheck + @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.TransferedStatus + "',',') X WHERE X.Item=complaints.IsTransferred)";

                    //ListingTypeCheck =
                    //    " AND complaints.Complaint_Computed_Hierarchy_Id <= " + listingModelBase.UserHierarchyId + @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.TransferedStatus + "',',') X WHERE X.Item=complaints.IsTransferred)";
                    if (listingModelBase.CheckIfExistInSrcId == 1 && !listingModelBase.IgnoreComputedHierarchyCheck)
                    {
                        ListingTypeCheck = ListingTypeCheck + @"
							and (complaints.SrcId1=" + listingModelBase.UserHierarchyId+@" OR complaints.SrcId2 = " + listingModelBase.UserHierarchyId+@"
							OR complaints.SrcId3=" + listingModelBase.UserHierarchyId + @" OR complaints.SrcId4 = " + listingModelBase.UserHierarchyId + @"
							OR complaints.SrcId5=" + listingModelBase.UserHierarchyId + @" OR complaints.SrcId6 = " + listingModelBase.UserHierarchyId + @"
							OR complaints.SrcId7=" + listingModelBase.UserHierarchyId + @" OR complaints.SrcId8 = " + listingModelBase.UserHierarchyId + @"
							OR complaints.SrcId9=" + listingModelBase.UserHierarchyId + @" OR complaints.SrcId10 = " + listingModelBase.UserHierarchyId + @")";
                    }
                    if (listingModelBase.CheckIfExistInUserSrcId == 1)
                    {
                        ListingTypeCheck = ListingTypeCheck + @"
							and (" + listingModelBase.UserDesignationHierarchyId + @"=0 
							OR complaints.UserSrcId1=" + listingModelBase.UserDesignationHierarchyId + @" OR complaints.UserSrcId2 = " + listingModelBase.UserDesignationHierarchyId + @"
							OR complaints.UserSrcId3=" + listingModelBase.UserDesignationHierarchyId + @" OR complaints.UserSrcId4 = " + listingModelBase.UserDesignationHierarchyId + @"
							OR complaints.UserSrcId5=" + listingModelBase.UserDesignationHierarchyId + @" OR complaints.UserSrcId6 = " + listingModelBase.UserDesignationHierarchyId + @"
							OR complaints.UserSrcId7=" + listingModelBase.UserDesignationHierarchyId + @" OR complaints.UserSrcId8 = " + listingModelBase.UserDesignationHierarchyId + @"
							OR complaints.UserSrcId9=" + listingModelBase.UserDesignationHierarchyId + @" OR complaints.UserSrcId10 = " + listingModelBase.UserDesignationHierarchyId + @")";
                    }
                    // users category checks
                    if (listingModelBase.UserCategoryId1 != null || listingModelBase.UserCategoryId2 != null)
                        // new check
                    {
                        if (listingModelBase.UserCategoryId1 == null)
                        {
                            usercategoryCheck = usercategoryCheck + " and complaints.UserCategoryId1 is null ";
                        }
                        else
                        {
                            usercategoryCheck = usercategoryCheck + " and complaints.UserCategoryId1 = " +
                                                listingModelBase.UserCategoryId1 + " ";
                        }

                        if (listingModelBase.UserCategoryId2 == null)
                        {
                            usercategoryCheck = usercategoryCheck + " and complaints.UserCategoryId2 is null ";
                        }
                        else
                        {
                            usercategoryCheck = usercategoryCheck + " and complaints.UserCategoryId2 = " +
                                                listingModelBase.UserCategoryId2 + " ";
                        }
                    }

                    //------------------- New User Category List Check -------------------
                    usercategoryCheck = "";
                    if (!UserCategoryModel.AreAllCategoriesNull(listingModelBase.ListUserCategory))
                    {
                        int count = 0;
                        foreach (UserCategoryModel userCategory in listingModelBase.ListUserCategory)
                        {
                            if (count == 0)
                            {
                                usercategoryCheck = usercategoryCheck + " and ( ";
                            }
                            usercategoryCheck = usercategoryCheck + "(";
                            if (userCategory.Parent_Category_Id == null) // when value is null
                            {
                                usercategoryCheck = usercategoryCheck + " complaints.UserCategoryId" +
                                                    userCategory.Category_Hierarchy + " is null";
                            }
                            else
                            {
                                usercategoryCheck = usercategoryCheck + " complaints.UserCategoryId" +
                                                    userCategory.Category_Hierarchy + " = " +
                                                    userCategory.Parent_Category_Id;
                            }


                            if (userCategory.Child_Category_Id == null) // when value is null
                            {
                                usercategoryCheck = usercategoryCheck + " and complaints.UserCategoryId" +
                                                    (userCategory.Category_Hierarchy + 1) + " is null";
                            }
                            else
                            {
                                usercategoryCheck = usercategoryCheck + " and complaints.UserCategoryId" +
                                                    (userCategory.Category_Hierarchy + 1) + " = " +
                                                    userCategory.Child_Category_Id;
                            }
                            usercategoryCheck = usercategoryCheck + ")";
                            if ((count + 1) < listingModelBase.ListUserCategory.Count)
                            {
                                usercategoryCheck = usercategoryCheck + " OR ";
                            }
                            else
                            {
                                usercategoryCheck = usercategoryCheck + " ) ";
                            }
                            count++;
                        }
                    }


                    //---------------------- End Category list Check ----------------

                }
                else if (listingModelBase.ListingType == 2) // Uptil my Hierarchy
                {
                    ComplaintTypeSelection = ComplaintTypeSelection +
                                                 ", complaints.Computed_Remaining_Time_To_Escalate as Computed_Remaining_Time_To_Escalate";

                    ListingTypeCheck = @" AND (complaints.MaxSrcId is null or complaints.MaxSrcId >=" + listingModelBase.UserHierarchyId+@")";
                    if (listingModelBase.UserCategoryId1 != null || listingModelBase.UserCategoryId2 != null)
                        // new check
                    {
                        if (listingModelBase.UserCategoryId1 != null)
                        {
                            usercategoryCheck = usercategoryCheck + " and complaints.UserCategoryId1 = " +
                                                listingModelBase.UserCategoryId1 + " ";
                        }
                        if (listingModelBase.UserCategoryId2 != null)
                        {
                            usercategoryCheck = usercategoryCheck + " and complaints.UserCategoryId2 = " +
                                                listingModelBase.UserCategoryId2 + " ";
                        }
                    }

                    //------------------- New User Category List Check -------------------
                    usercategoryCheck = "";
                    if (!UserCategoryModel.AreAllCategoriesNull(listingModelBase.ListUserCategory))
                    {
                        int count = 0;
                        foreach (UserCategoryModel userCategory in listingModelBase.ListUserCategory)
                        {
                            if (count == 0)
                            {
                                usercategoryCheck = usercategoryCheck + " and ( ";
                            }
                            usercategoryCheck = usercategoryCheck + "(";
                            /*if (userCategory.Parent_Category_Id == null) // when value is null
                            {
                                usercategoryCheck = usercategoryCheck + " complaints.UserCategoryId" +
                                                    userCategory.Category_Hierarchy + " is null";
                            }*/
                            if (userCategory.Parent_Category_Id != null)
                            {
                                usercategoryCheck = usercategoryCheck + " complaints.UserCategoryId" +
                                                    userCategory.Category_Hierarchy + " = " +
                                                    userCategory.Parent_Category_Id;
                            }

                            /*
                            if (userCategory.Child_Category_Id == null) // when value is null
                            {
                                usercategoryCheck = usercategoryCheck + " and complaints.UserCategoryId" +
                                                    (userCategory.Category_Hierarchy + 1) + " is null";
                            }*/
                            if ((userCategory.Child_Category_Id != null))
                            {
                                usercategoryCheck = usercategoryCheck + " and complaints.UserCategoryId" +
                                                    (userCategory.Category_Hierarchy + 1) + " = " +
                                                    userCategory.Child_Category_Id;
                            }
                            usercategoryCheck = usercategoryCheck + ")";
                            if ((count + 1) < listingModelBase.ListUserCategory.Count)
                            {
                                usercategoryCheck = usercategoryCheck + " OR ";
                            }
                            else
                            {
                                usercategoryCheck = usercategoryCheck + " ) ";
                            }
                            count++;
                        }
                    }


                    //---------------------- End Category list Check ----------------

                    //ListingTypeCheck = "";
                }

                ComplaintTypeHierarchyCheck =
                    @" AND EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.Status + @"',',') X WHERE X.Item=complaints.Complaint_Computed_Status_Id)" +
                    ListingTypeCheck;
                // AND complaints.Complaint_Computed_Hierarchy_Id <= @UserHierarchyId';


                

            }
            else if (listingModelBase.ComplaintType == 2 || listingModelBase.ComplaintType == 3)
                // Suggestion and Inquiry
            {
                ComplaintTypeSelection = "";
                ComplaintTypeHierarchyCheck = "";

                // testing start
               /* if (listingModelBase.CheckIfExistInSrcId == 1)
                {
                    ListingTypeCheck = ListingTypeCheck + @"
							and (complaints.SrcId1=" + listingModelBase.UserHierarchyId + @" OR complaints.SrcId2 = " + listingModelBase.UserHierarchyId + @"
							OR complaints.SrcId3=" + listingModelBase.UserHierarchyId + @" OR complaints.SrcId4 = " + listingModelBase.UserHierarchyId + @"
							OR complaints.SrcId5=" + listingModelBase.UserHierarchyId + @" OR complaints.SrcId6 = " + listingModelBase.UserHierarchyId + @"
							OR complaints.SrcId7=" + listingModelBase.UserHierarchyId + @" OR complaints.SrcId8 = " + listingModelBase.UserHierarchyId + @"
							OR complaints.SrcId9=" + listingModelBase.UserHierarchyId + @" OR complaints.SrcId10 = " + listingModelBase.UserHierarchyId + @")";
                }
                if (listingModelBase.CheckIfExistInUserSrcId == 1)
                {
                    ListingTypeCheck = ListingTypeCheck + @"
							and (" + listingModelBase.UserDesignationHierarchyId + @"=0 
							OR complaints.UserSrcId1=" + listingModelBase.UserDesignationHierarchyId + @" OR complaints.UserSrcId2 = " + listingModelBase.UserDesignationHierarchyId + @"
							OR complaints.UserSrcId3=" + listingModelBase.UserDesignationHierarchyId + @" OR complaints.UserSrcId4 = " + listingModelBase.UserDesignationHierarchyId + @"
							OR complaints.UserSrcId5=" + listingModelBase.UserDesignationHierarchyId + @" OR complaints.UserSrcId6 = " + listingModelBase.UserDesignationHierarchyId + @"
							OR complaints.UserSrcId7=" + listingModelBase.UserDesignationHierarchyId + @" OR complaints.UserSrcId8 = " + listingModelBase.UserDesignationHierarchyId + @"
							OR complaints.UserSrcId9=" + listingModelBase.UserDesignationHierarchyId + @" OR complaints.UserSrcId10 = " + listingModelBase.UserDesignationHierarchyId + @")";
                }*/
                // users category checks

                if (listingModelBase.UserCategoryId1 != null || listingModelBase.UserCategoryId2 != null) // new check
                {
                    int count = 0;
                    foreach (UserCategoryModel userCategory in listingModelBase.ListUserCategory)
                    {
                        if (count == 0)
                        {
                            usercategoryCheck = usercategoryCheck + " and ( ";
                        }
                        usercategoryCheck = usercategoryCheck + "(";
                        if (userCategory.Parent_Category_Id == null) // when value is null
                        {
                            usercategoryCheck = usercategoryCheck + " complaints.UserCategoryId" +
                                                userCategory.Category_Hierarchy + " is null";
                        }
                        else
                        {
                            usercategoryCheck = usercategoryCheck + " complaints.UserCategoryId" +
                                                userCategory.Category_Hierarchy + " = " +
                                                userCategory.Parent_Category_Id;
                        }


                        if (userCategory.Child_Category_Id == null) // when value is null
                        {
                            usercategoryCheck = usercategoryCheck + " and complaints.UserCategoryId" +
                                                (userCategory.Category_Hierarchy + 1) + " is null";
                        }
                        else
                        {
                            usercategoryCheck = usercategoryCheck + " and complaints.UserCategoryId" +
                                                (userCategory.Category_Hierarchy + 1) + " = " +
                                                userCategory.Child_Category_Id;
                        }
                        usercategoryCheck = usercategoryCheck + ")";
                        if ((count + 1) < listingModelBase.ListUserCategory.Count)
                        {
                            usercategoryCheck = usercategoryCheck + " OR ";
                        }
                        else
                        {
                            usercategoryCheck = usercategoryCheck + " ) ";
                        }
                        count++;
                    }
                }


                //------------------- New User Category List Check -------------------
                usercategoryCheck = "";
                if (!UserCategoryModel.AreAllCategoriesNull(listingModelBase.ListUserCategory))
                {
                    foreach (UserCategoryModel userCategory in listingModelBase.ListUserCategory)
                    {
                        if (userCategory.Parent_Category_Id == null) // when value is null
                        {
                            usercategoryCheck = usercategoryCheck + " and complaints.UserCategoryId" +
                                                userCategory.Category_Hierarchy + " is null";
                        }
                        else
                        {
                            usercategoryCheck = usercategoryCheck + " and complaints.UserCategoryId" +
                                                userCategory.Category_Hierarchy + " = " +
                                                userCategory.Parent_Category_Id;
                        }


                        if (userCategory.Child_Category_Id == null) // when value is null
                        {
                            usercategoryCheck = usercategoryCheck + " and complaints.UserCategoryId" +
                                                (userCategory.Category_Hierarchy + 1) + " is null";
                        }
                        else
                        {
                            usercategoryCheck = usercategoryCheck + " and complaints.UserCategoryId" +
                                                (userCategory.Category_Hierarchy + 1) + " = " +
                                                userCategory.Child_Category_Id;
                        }

                    }
                }


                //---------------------- End Category list Check ----------------

                //testing end
            }

            //Dictionary<string, object> dictParams = new Dictionary<string, object>();
            //dictParams.Add("listingModelBase.SelectionFields", listingModelBase.SelectionFields);
            //dictParams.Add("ComplaintTypeSelection", ComplaintTypeSelection);
            //dictParams.Add("listingModelBase.OrderByColumnName", listingModelBase.OrderByColumnName);
            //dictParams.Add("listingModelBase.OrderByDirection", listingModelBase.OrderByDirection);
            //dictParams.Add("listingModelBase.InnerJoinLogic", listingModelBase.InnerJoinLogic);
            //dictParams.Add("listingModelBase.ComplaintType", listingModelBase.ComplaintType);
            //dictParams.Add("listingModelBase.Campaign", listingModelBase.Campaign);
            //dictParams.Add("listingModelBase.From", listingModelBase.From);
            //dictParams.Add("listingModelBase.To", listingModelBase.To);
            //dictParams.Add("listingModelBase.Category", listingModelBase.Category);
            //dictParams.Add("usercategoryCheck", usercategoryCheck);
            //dictParams.Add("ComplaintTypeHierarchyCheck", ComplaintTypeHierarchyCheck);
            //dictParams.Add("MulticolumnSearchQuery", MulticolumnSearchQuery);
            //dictParams.Add("HierarchyCheck", HierarchyCheck);
            //dictParams.Add("listingModelBase.StartRow", listingModelBase.StartRow);
            //dictParams.Add("listingModelBase.EndRow", listingModelBase.EndRow);

            Dictionary<string, object> dictParams = new Dictionary<string, object>();
            dictParams.Add("@OrderByColumnName", listingModelBase.OrderByColumnName.ToDbObj());
            dictParams.Add("@OrderByDirection", listingModelBase.OrderByDirection.ToDbObj());
            dictParams.Add("@InnerJoinLogic", listingModelBase.InnerJoinLogic.ToDbObj());
            dictParams.Add("@ComplaintType", listingModelBase.ComplaintType.ToDbObj());
            dictParams.Add("@Campaign", listingModelBase.Campaign.ToDbObj());
            dictParams.Add("@From", listingModelBase.From.ToDbObj());
            dictParams.Add("@To", listingModelBase.To.ToDbObj());
            dictParams.Add("@Category", listingModelBase.Category.ToDbObj());
            dictParams.Add("@UsercategoryCheck", usercategoryCheck.ToDbObj());
            dictParams.Add("@ComplaintTypeHierarchyCheck", ComplaintTypeHierarchyCheck);
            dictParams.Add("@MulticolumnSearchQuery", MulticolumnSearchQuery);
            dictParams.Add("@HierarchyCheck", HierarchyCheck);
            dictParams.Add("@StartRow", listingModelBase.StartRow);
            dictParams.Add("@EndRow", listingModelBase.EndRow);


            if (listingModelBase.SpType == "Listing")
            {
                FinalSQL = @"
					    SELECT * from (SELECT " + listingModelBase.SelectionFields +
                           @"complaints.Id as ComplaintId,(CAST(complaints.Compaign_Id AS VARCHAR(10))+'-'+CAST(complaints.Id AS NVARCHAR(10))) AS Id, complaints.Campaign_Name Campaign_Name,complaints.Person_Name Person_Name,complaints.Province_Id Province_Id,complaints.Province_Name Province_Name,complaints.Division_Id Division_Id,complaints.Division_Name Division_Name,complaints.District_Id District_Id, complaints.District_Name District_Name, complaints.Tehsil_Id Tehsil_Id,Ref_Complaint_Id, complaints.Tehsil_Name Tehsil_Name, complaints.UnionCouncil_Id UnionCouncil_Id,complaints.UnionCouncil_Name UnionCouncil_Name,
				    CONVERT(VARCHAR(10),complaints.Created_Date,120)+RIGHT(CONVERT(VARCHAR, complaints.Created_Date, 100), 7) Created_Date,complaints.Complaint_Category_Name Complaint_Category_Name " +
                           ComplaintTypeSelection + @", count(*)  OVER() AS Total_Rows,
				    ROW_NUMBER() OVER (ORDER BY " + listingModelBase.OrderByColumnName + " " + listingModelBase.OrderByDirection +
                           @" ) AS RowNum
				    FROM pitb.Complaints complaints " + listingModelBase.InnerJoinLogic + @"
			
				    --inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				    WHERE complaints.Complaint_Type = " + listingModelBase.ComplaintType +
                           string.Format(" and complaints.Compaign_Id in ({0})", listingModelBase.Campaign) +
                    @"AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + listingModelBase.From + @"' AND '" +
                           listingModelBase.To + @"' ) " +
                    string.Format(" and complaints.Complaint_Category in ({0}) ", listingModelBase.Category)
                     + usercategoryCheck + ComplaintTypeHierarchyCheck
                           + MulticolumnSearchQuery + @" 
				    -- My code
			
				    --and complaints.Province_Id = @ProvinceId) as tbl
				     " + @HierarchyCheck + @") as tbl
			
				    WHERE tbl.RowNum BETWEEN " + listingModelBase.StartRow + @" AND " + listingModelBase.EndRow + @"
		        ";


        //        FinalSQL = @"SELECT * from (SELECT complaints.Id as ComplaintId, count(*)  OVER() AS Total_Rows,
				    //ROW_NUMBER() OVER (ORDER BY @OrderByColumnName @OrderByDirection  ) AS RowNum
				    //FROM pitb.Complaints complaints @InnerJoinLogic 
				    //WHERE complaints.Complaint_Type = @ComplaintType 
        //            AND complaints.Compaign_Id in (@Campaign)
        //            AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN @From AND @To ) 
        //            AND complaints.Complaint_Category in (@Category) 
        //            @UsercategoryCheck  @ComplaintTypeHierarchyCheck 
        //            @MulticolumnSearchQuery @HierarchyCheck) as tbl
			
				    //WHERE tbl.RowNum BETWEEN @StartRow AND @EndRow";
        //        FinalSQL = QueryHelper.GetParamsReplacedQuery(FinalSQL, dictParams);

                

                //Custom Code
                //Dictionary<string, object> dictParams = new Dictionary<string, object>();
                //dictParams.Add("listingModelBase.SelectionFields", listingModelBase.SelectionFields);
                //dictParams.Add("ComplaintTypeSelection", ComplaintTypeSelection);
                //dictParams.Add("listingModelBase.OrderByColumnName", listingModelBase.OrderByColumnName);
                //dictParams.Add("listingModelBase.OrderByDirection", listingModelBase.OrderByDirection);
                //dictParams.Add("listingModelBase.InnerJoinLogic", listingModelBase.InnerJoinLogic);
                //dictParams.Add("listingModelBase.ComplaintType", listingModelBase.ComplaintType);
                //dictParams.Add("listingModelBase.Campaign", listingModelBase.Campaign);
                //dictParams.Add("listingModelBase.From", listingModelBase.From);
                //dictParams.Add("listingModelBase.To", listingModelBase.To);
                //dictParams.Add("listingModelBase.Category", listingModelBase.Category);
                //dictParams.Add("usercategoryCheck", usercategoryCheck);
                //dictParams.Add("ComplaintTypeHierarchyCheck", ComplaintTypeHierarchyCheck);
                //dictParams.Add("MulticolumnSearchQuery", MulticolumnSearchQuery);
                //dictParams.Add("HierarchyCheck", HierarchyCheck);
                //dictParams.Add("listingModelBase.StartRow", listingModelBase.StartRow);
                //dictParams.Add("listingModelBase.EndRow", listingModelBase.EndRow);

                //if (listingModelBase.ListingType == 1) // Assigned to me 
                //{
                    
                //}
                //else if (listingModelBase.ListingType == 2)
                //{

                //}

                //FinalSQL = QueryHelper.GetFinalQuery("ComplaintsListingAssignedToMe", Config.ConfigType.Query, dictParams);
                //FinalSQL = QueryHelper.GetQuery(null, dictParams);
                //End Custom Code
            }

            if (listingModelBase.SpType == "UserComplaintsList")
            {
                FinalSQL = @"
					    SELECT * from (SELECT "+ listingModelBase.SelectionFields +
				    " FROM pitb.Complaints complaints " + listingModelBase.InnerJoinLogic + @"
			
				    --inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				    WHERE complaints.Complaint_Type = " + listingModelBase.ComplaintType +
                           @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.Campaign +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)
				    --AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + listingModelBase.From + @"' AND '" +listingModelBase.To + @"' ) 
				    AND EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.Category +
                           @"',',') X WHERE X.Item=complaints.Complaint_Category)
				
				   " + usercategoryCheck + ComplaintTypeHierarchyCheck
                           + MulticolumnSearchQuery + @" 
				    -- My code
			
				    --and complaints.Province_Id = @ProvinceId) as tbl
				     " + @HierarchyCheck + @") as tbl";
            }

            if (listingModelBase.SpType == "MobileListingWithDateFilters")
            {
                FinalSQL = @"
					     SELECT * from (SELECT " + listingModelBase.SelectionFields +
                           @"complaints.Id as Complaint_Id,(CAST(complaints.Compaign_Id AS VARCHAR(10))+'-'+CAST(complaints.Id AS NVARCHAR(10))) AS Id, complaints.Campaign_Name Campaign_Name,complaints.Person_Name Person_Name, complaints.Person_Cnic,complaints.Person_Contact,
				complaints.District_Name District_Name, complaints.Tehsil_Name Tehsil_Name,complaints.UnionCouncil_Name UnionCouncil_Name, complaints.Computed_Remaining_Time_To_Escalate,complaints.StatusReopenedCount,
			 Created_Date,complaints.Complaint_Category_Name Complaint_Category_Name, complaints.Complaint_SubCategory_Name Complaint_SubCategory_Name,complaints.Complaint_Remarks,complaints.Complainant_Remark_Id,complaints.Complainant_Remark_Str " +
                           "" + @", count(*)  OVER() AS Total_Rows,
				    ROW_NUMBER() OVER (ORDER BY complaints.Created_Date DESC  " +
                           @" ) AS RowNum
				    FROM pitb.Complaints complaints " + listingModelBase.InnerJoinLogic + @"
			
				    --inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				    WHERE complaints.Complaint_Type = " + listingModelBase.ComplaintType +
                           @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.Campaign +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)
				    AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + listingModelBase.From + @"' AND '" +
                           listingModelBase.To + @"' ) 
				    AND EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.Category +
                           @"',',') X WHERE X.Item=complaints.Complaint_Category)
				
				   " + usercategoryCheck + ComplaintTypeHierarchyCheck
                           + MulticolumnSearchQuery + @" 
				    -- My code
			
				    --and complaints.Province_Id = @ProvinceId) as tbl
				     " + @HierarchyCheck + @") as tbl
			
				    WHERE tbl.RowNum BETWEEN " + listingModelBase.StartRow + @" AND " + listingModelBase.EndRow + @"
		        ";
            }
            else if (listingModelBase.SpType == "ListingExpiring")
            {
                FinalSQL = @"
					    SELECT * from (SELECT " + listingModelBase.SelectionFields +
                           @"complaints.Id as ComplaintId,(CAST(complaints.Compaign_Id AS VARCHAR(10))+'-'+CAST(complaints.Id AS NVARCHAR(10))) AS Id, complaints.Campaign_Name Campaign_Name,complaints.Person_Name Person_Name, complaints.District_Name District_Name, complaints.Tehsil_Name Tehsil_Name,complaints.UnionCouncil_Name UnionCouncil_Name,
				    CONVERT(VARCHAR(10),complaints.Created_Date,120) Created_Date,complaints.Complaint_Category_Name Complaint_Category_Name " +
                           ComplaintTypeSelection + @", count(*)  OVER() AS Total_Rows,
				    ROW_NUMBER() OVER (ORDER BY " + listingModelBase.OrderByColumnName + " " + listingModelBase.OrderByDirection +
                           @" ) AS RowNum
				    FROM pitb.Complaints complaints " + listingModelBase.InnerJoinLogic + @"
			
				    --inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				    WHERE complaints.Complaint_Type = " + listingModelBase.ComplaintType +
                           @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.Campaign +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)
				    AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + listingModelBase.From + @"' AND '" +
                           listingModelBase.To + @"' ) 
				    AND EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.Category +
                           @"',',') X WHERE X.Item=complaints.Complaint_Category) 
                    AND complaints.Computed_Remaining_Time_Percentage > 0 and complaints.Computed_Remaining_Time_Percentage <= 30  
				
				   " + usercategoryCheck + ComplaintTypeHierarchyCheck
                           + MulticolumnSearchQuery + @" 
				    -- My code
			
				    --and complaints.Province_Id = @ProvinceId) as tbl
				     " + @HierarchyCheck + @") as tbl
			
				    WHERE tbl.RowNum BETWEEN " + listingModelBase.StartRow + @" AND " + listingModelBase.EndRow + @"
		        ";
            }
            if (listingModelBase.SpType == "ComplaintsAssignedToUser")
            {
                FinalSQL = @"
					    SELECT  complaints.Id as ComplaintId
				    FROM pitb.Complaints complaints " + listingModelBase.InnerJoinLogic + @"
			
				    --inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				    WHERE complaints.Complaint_Type = " + listingModelBase.ComplaintType +
                           @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.Campaign +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)
				    AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + listingModelBase.From + @"' AND '" +
                           listingModelBase.To + @"' ) 
				    AND EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.Category +
                           @"',',') X WHERE X.Item=complaints.Complaint_Category)
				
				   " + usercategoryCheck + ComplaintTypeHierarchyCheck
                           + MulticolumnSearchQuery + @" 
				    -- My code
			
				    --and complaints.Province_Id = @ProvinceId) as tbl
				     " + @HierarchyCheck + @" 
                   ORDER BY complaints.Id 

				   -- WHERE tbl.RowNum BETWEEN " + listingModelBase.StartRow + @" AND " + listingModelBase.EndRow + @"
		        ";
            }
            else if (listingModelBase.SpType == "ExcelReport")
            {
                //FinalSQL = QueryHelper.GetFinalQuery("ComplaintsListingAssignedToMe", Config.ConfigType.Query, dictParams);
                

                FinalSQL = @"
				SELECT * FROM(
				SELECT 
					"+ listingModelBase.SelectionFields +@"
					 FROM pitb.Complaints complaints " + listingModelBase.InnerJoinLogic + @"

					WHERE 
							complaints.Complaint_Type = " + listingModelBase.ComplaintType + @"
							AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + listingModelBase.From + @"' AND '" + listingModelBase.To +
                           @"' )
							AND EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.Campaign +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)
							AND EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.Category +
                           @"',',') X WHERE X.Item=complaints.Complaint_Category)
							" + ComplaintTypeHierarchyCheck
                           + @MulticolumnSearchQuery
                           + @HierarchyCheck + @"
							
							)Data ORDER BY Data.[Created Date] DESC";
            }
            else if (listingModelBase.SpType == "DashboardLabelsStausWise")
            {
                FinalSQL = @"
					
				select * from (SELECT statuses.Id AS Id, statuses.Status AS Name, COUNT(1) AS Count --, (CASE WHEN COUNT(1) = 1 THEN 0 else COUNT(1) END) AS Count 
				FROM PITB.Statuses statuses inner JOIN pitb.Complaints complaints ON complaints.Complaint_Computed_Status_Id = statuses.Id
			    " + listingModelBase.InnerJoinLogic + @"
				--inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				WHERE complaints.Complaint_Type = " + listingModelBase.ComplaintType+@" and EXISTS(SELECT 1 FROM dbo.SplitString('" +
                           listingModelBase.Campaign + @"',',') X WHERE X.Item=complaints.Compaign_Id)
				AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + listingModelBase.From+@"' AND '" + listingModelBase.To+@"' ) 
				AND EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.Category +
                           @"',',') X WHERE X.Item=complaints.Complaint_Category)
				" + usercategoryCheck + @ComplaintTypeHierarchyCheck
                           + @MulticolumnSearchQuery
                           + @HierarchyCheck + @" 
				 GROUP BY statuses.Id, statuses.Status
				 ) as tbl
				 
		        ";
            }
            else if (listingModelBase.SpType == "DashboardLabelsComplaintSrc")
            {
                FinalSQL = @"
					
				select * from (SELECT complaints.ComplaintSrc AS Id, COUNT(1) AS Count --, (CASE WHEN COUNT(1) = 1 THEN 0 else COUNT(1) END) AS Count 
				FROM pitb.Complaints complaints " + listingModelBase.InnerJoinLogic + @"
			
				--inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				WHERE complaints.Complaint_Type = " + listingModelBase.ComplaintType + @" and EXISTS(SELECT 1 FROM dbo.SplitString('" +
                           listingModelBase.Campaign + @"',',') X WHERE X.Item=complaints.Compaign_Id)
				AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + listingModelBase.From + @"' AND '" + listingModelBase.To +
                           @"' ) 
				AND EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.Category +
                           @"',',') X WHERE X.Item=complaints.Complaint_Category)
				
				" + usercategoryCheck + @ComplaintTypeHierarchyCheck
                           + @MulticolumnSearchQuery
                           + @HierarchyCheck + @" 
				 GROUP BY complaints.ComplaintSrc
				 ) as tbl
				 
		        ";
            }
            else if (listingModelBase.SpType == "StatusWiseUserComplaints")
            {
                FinalSQL = @"
					    --SELECT " + listingModelBase.SelectionFields +
                           @"--complaints.Id as ComplaintId,(CAST(complaints.Compaign_Id AS VARCHAR(10))+'-'+CAST(complaints.Id AS NVARCHAR(10))) AS Id, complaints.Campaign_Name Campaign_Name,complaints.Person_Name Person_Name, complaints.District_Name District_Name, complaints.Tehsil_Name Tehsil_Name,complaints.UnionCouncil_Name UnionCouncil_Name,
				    --CONVERT(VARCHAR(10),complaints.Created_Date,120) Created_Date,complaints.Complaint_Category_Name Complaint_Category_Name
                    SELECT " + listingModelBase.UserId + @" as User_Id, complaints.Complaint_Computed_Status_Id, complaints.Complaint_Computed_Status, COUNT(*) Count 
				    FROM pitb.Complaints complaints  " + listingModelBase.InnerJoinLogic + @"
			
				    --inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				    WHERE complaints.Complaint_Type = " + listingModelBase.ComplaintType +
                           @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.Campaign +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)
				    AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + listingModelBase.From + @"' AND '" +
                           listingModelBase.To + @"' ) 
				    AND EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.Category +
                           @"',',') X WHERE X.Item=complaints.Complaint_Category)
				
				   " + usercategoryCheck + ComplaintTypeHierarchyCheck
                           + MulticolumnSearchQuery + @" 
				    -- My code
			
				    --and complaints.Province_Id = @ProvinceId) as tbl
				     " + @HierarchyCheck + @" 
                    GROUP BY complaints.Complaint_Computed_Status_Id, complaints.Complaint_Computed_Status
		        ";
            }

            else if (listingModelBase.SpType == "CategoryWiseStatusComplaintCounts")
            {
                FinalSQL = @"

                    SELECT " + listingModelBase.SelectionFields + @", complaints.Complaint_Computed_Status_Id, complaints.Complaint_Computed_Status, COUNT(*) Count 
				    FROM pitb.Complaints complaints  " + listingModelBase.InnerJoinLogic + @"
			
				    --inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				    WHERE complaints.Complaint_Type = " + listingModelBase.ComplaintType +
                           @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.Campaign +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)
				    AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + listingModelBase.From + @"' AND '" +
                           listingModelBase.To + @"' ) 
				   " + usercategoryCheck + ComplaintTypeHierarchyCheck
                           + MulticolumnSearchQuery + @" 
				    -- My code
			
				    --and complaints.Province_Id = @ProvinceId) as tbl
				     " + @HierarchyCheck + 
                      " Group by "+ listingModelBase.GroupByLogic+ "";
            }
            else if (listingModelBase.SpType == "OverDueComplaintsSummary")
            {
                FinalSQL = @"

                    SELECT " + listingModelBase.SelectionFields +@"
				    FROM pitb.Complaints complaints  " + listingModelBase.InnerJoinLogic + @"
			
				    --inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				    WHERE complaints.Complaint_Type = " + listingModelBase.ComplaintType +
                           @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.Campaign +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)
				    AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + listingModelBase.From + @"' AND '" +
                           listingModelBase.To + @"' ) 
				   " + usercategoryCheck + ComplaintTypeHierarchyCheck
                           + MulticolumnSearchQuery + @" 
				    -- My code
			
				    --and complaints.Province_Id = @ProvinceId) as tbl
				     " + @HierarchyCheck + "";
            }
            else if (listingModelBase.SpType == "RegionStatusWiseSummary")
            {
                FinalSQL = @"

                    SELECT " + listingModelBase.SelectionFields + @"
				    FROM pitb.Complaints complaints  " + listingModelBase.InnerJoinLogic + @"
			
				    --inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				    WHERE complaints.Complaint_Type = " + listingModelBase.ComplaintType +
                           @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.Campaign +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)
				    AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + listingModelBase.From + @"' AND '" +
                           listingModelBase.To + @"' ) 
				   " + usercategoryCheck + ComplaintTypeHierarchyCheck
                           + MulticolumnSearchQuery + @" 
				    -- My code
			
				    --and complaints.Province_Id = @ProvinceId) as tbl
				     " + @HierarchyCheck + 
                    " Group by "+ listingModelBase.GroupByLogic+ "";
            
            }


            else if (listingModelBase.SpType == "DynamicFieldsWiseCounts")
            {
                FinalSQL = @"
					    SELECT " + listingModelBase.SelectionFields + @", dynamicFields.CategoryTypeId,dynamicFields.FieldValue, COUNT(1) Count
                        --complaints.id,dynamicFields.CategoryTypeId , dynamicFields.FieldName,dynamicFields.FieldValue
                        FROM pitb.Complaints complaints INNER JOIN pitb.Dynamic_ComplaintFields dynamicFields
                        ON complaints.Id=dynamicFields.ComplaintId 
                        WHERE dynamicFields.ControlId = " + listingModelBase.DynamicFieldsControlId + @" AND complaints.Complaint_Type = 1 AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + listingModelBase.From + @"' AND '" +
                           listingModelBase.To + @"' ) " + @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.Campaign +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)"+listingModelBase.WhereLogic+@"
                        GROUP BY "+listingModelBase.GroupByLogic+", dynamicFields.CategoryTypeId,dynamicFields.FieldValue";
            }
            return FinalSQL;

        }
    }
}