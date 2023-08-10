using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using PITB.CMS_Common.Handler.Configuration;
using PITB.CMS_Common.Helper;
using PITB.CMS_Common.Helper.Database;
using PITB.CMS_Common.Models.Custom;

namespace PITB.CMS_Common.Handler.Complaint
{
    public class StakeholderListingLogic
    {

        public static string GetListingQuery(ListingParamsModelBase listingModelBase)
        {
            string HierarchyCheck = "";
            string UserDesignationHierarchyCheck;
            string FinalSQL = "";
            string MulticolumnSearchQuery;

            //MulticolumnSearchQuery = listingModelBase.WhereOfMultiSearchParametrized + listingModelBase.WhereLogic;
            MulticolumnSearchQuery = listingModelBase.WhereOfMultiSearch + listingModelBase.WhereLogic;
            /*if ((Config.StakeholderComplaintListingType) listingModelBase.ListingType ==
                Config.StakeholderComplaintListingType.UptilMyHierarchy)
            {
                listingModelBase.UserHierarchyId++;
            }*/
            switch ((Config.Hierarchy)listingModelBase.UserHierarchyId)
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

            if ((Config.ComplaintType)listingModelBase.ComplaintType == Config.ComplaintType.Complaint) // Complaint
            {
                ComplaintTypeSelection =
                    ",Complaint_Computed_Hierarchy_Id, Complaint_Computed_User_Hierarchy_Id, UserCategoryId1, UserCategoryId2, complaints.Complaint_Computed_Status as Complaint_Computed_Status, complaints.Complaint_Computed_Hierarchy  as Complaint_Computed_Hierarchy,  complaints.FollowupCount ";

                if ((Config.StakeholderComplaintListingType)listingModelBase.ListingType ==
                    Config.StakeholderComplaintListingType.AssignedToMe) //  Assigned To Me
                {
                    if (listingModelBase.UserDesignationHierarchyId != null) //AND @UserDesignationHierarchyId<>0)
                    {
                        ComplaintTypeSelection = ComplaintTypeSelection +
                                                 ", complaints.Computed_Remaining_Time_To_Escalate as Computed_Remaining_Time_To_Escalate";
                        HierarchyCheck = HierarchyCheck +
                                         " and complaints.Complaint_Computed_User_Hierarchy_Id >= " + listingModelBase.UserDesignationHierarchyId + "";
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

                    ListingTypeCheck = @" AND (complaints.MaxSrcId is null or complaints.MaxSrcId >=" + listingModelBase.UserHierarchyId + @")";
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
                           @") AS RowNum
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
					    SELECT * from (SELECT " + listingModelBase.SelectionFields +
                    " FROM pitb.Complaints complaints " + listingModelBase.InnerJoinLogic + @"
			
				    --inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				    WHERE complaints.Complaint_Type = " + listingModelBase.ComplaintType +
                           @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.Campaign +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)
				    --AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + listingModelBase.From + @"' AND '" + listingModelBase.To + @"' ) 
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
                           @" DESC) AS RowNum
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

                //------if tag query exists------

                //------ end tag exit-------------



                FinalSQL = @"
				SELECT * FROM(
				SELECT 
					" + listingModelBase.SelectionFields + @"
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

				WHERE complaints.Complaint_Type = " + listingModelBase.ComplaintType + @" and EXISTS(SELECT 1 FROM dbo.SplitString('" +
                           listingModelBase.Campaign + @"',',') X WHERE X.Item=complaints.Compaign_Id)
				AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + listingModelBase.From + @"' AND '" + listingModelBase.To + @"' ) 
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
                      " Group by " + listingModelBase.GroupByLogic + "";
            }
            else if (listingModelBase.SpType == "OverDueComplaintsSummary")
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
                    " Group by " + listingModelBase.GroupByLogic + "";

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
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)" + listingModelBase.WhereLogic + @"
                        GROUP BY " + listingModelBase.GroupByLogic + ", dynamicFields.CategoryTypeId,dynamicFields.FieldValue";
            }
            return FinalSQL;

        }
        public static Dictionary<string, object> GetQueryParams(ListingParamsModelBase listingModelBase)
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
            switch ((Config.Hierarchy)listingModelBase.UserHierarchyId)
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

            if ((Config.ComplaintType)listingModelBase.ComplaintType == Config.ComplaintType.Complaint) // Complaint
            {
                ComplaintTypeSelection =
                    ",Complaint_Computed_Hierarchy_Id, Complaint_Computed_User_Hierarchy_Id, UserCategoryId1, UserCategoryId2, complaints.Complaint_Computed_Status as Complaint_Computed_Status, complaints.Complaint_Computed_Hierarchy  as Complaint_Computed_Hierarchy,  complaints.FollowupCount ";

                if ((Config.StakeholderComplaintListingType)listingModelBase.ListingType ==
                    Config.StakeholderComplaintListingType.AssignedToMe) //  Assigned To Me
                {
                    if (listingModelBase.UserDesignationHierarchyId != null) //AND @UserDesignationHierarchyId<>0)
                    {
                        ComplaintTypeSelection = ComplaintTypeSelection +
                                                 ", complaints.Computed_Remaining_Time_To_Escalate as Computed_Remaining_Time_To_Escalate";
                        HierarchyCheck = HierarchyCheck +
                                         " and complaints.Complaint_Computed_User_Hierarchy_Id >= " + listingModelBase.UserDesignationHierarchyId + "";
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

                    ListingTypeCheck = @" AND (complaints.MaxSrcId is null or complaints.MaxSrcId >=" + listingModelBase.UserHierarchyId + @")";
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
            dictParams.Add("@usercategoryCheck", usercategoryCheck.ToDbObj());
            dictParams.Add("@ComplaintTypeHierarchyCheck", ComplaintTypeHierarchyCheck);
            dictParams.Add("@MulticolumnSearchQuery", MulticolumnSearchQuery);
            dictParams.Add("@HierarchyCheck", HierarchyCheck);
            dictParams.Add("@StartRow", listingModelBase.StartRow);
            dictParams.Add("@EndRow", listingModelBase.EndRow);

            ////----------  new columns ----------/////
            dictParams.Add("@SpType", listingModelBase.SpType);
            dictParams.Add("@SelectionFields", listingModelBase.SelectionFields);
            dictParams.Add("@ComplaintTypeSelection", ComplaintTypeSelection.ToDbObj());
            dictParams.Add("@UserId", listingModelBase.UserId.ToDbObj());
            dictParams.Add("@GroupByLogic", listingModelBase.GroupByLogic.ToDbObj());
            dictParams.Add("@WhereLogic", listingModelBase.WhereLogic.ToDbObj());



            return dictParams;
        }
        public static /*Dictionary<string, object>*/ dynamic GetQueryWithParams ( dynamic dParam /*ListingParamsModelBase listingModelBase*/)
        {
            dynamic dToRet = new ExpandoObject();
            
            dToRet.dictParam = dParam.dictParam;
            dToRet.queryWhereClause = "";
            dToRet.querySelectClause = "";

            Dictionary<string, object> dictParam = dToRet.dictParam;
            string query = dToRet.queryWhereClause;

            List<int> listCategories = dParam.listCategories;
            List<int> listTransferedStatus = dParam.listTransferedStatus;
            List<int> listComplaintStatuses = dParam.listComplaintStatuses;
            //string whereOfMultisearch = dParam.whereOfMultiSearchParametrized;
            //string whereLogic = dParam.whereToInject;

            int complaintType = (int)dParam.complaintType;
            int listingType = (int)dParam.listingType;
            int? hierarchyId = dParam.hierarchyId;
            int? userHierarchyId = dParam.userHierarchyId;
            int? userCategoryId1 = dParam.userCategoryId1;
            int? userCategoryId2 = dParam.userCategoryId2;

            bool ignoreComputedHierarchyCheck = dParam.ignoreComputedHierarchyCheck;
            int checkIfExistInSrcId = dParam.checkIfExistInSrcId;
            
            bool canComputeComplaintType = dParam.canComputeComplaintType;
            bool canComputeCampaignId = dParam.canComputeCampaignId;
            bool canComputeDateRange = dParam.canComputeDateRange;
            bool canComputeHierarchy = dParam.canComputeHierarchy;
            bool canComputeUserHierarchy = dParam.canComputeUserHierarchy;
            bool canComputeUserCategory = dParam.canComputeUserCategory;
            bool canComputeStatus = dParam.canComputeStatus;
            bool canComputeListingType = dParam.canComputeListingType;
            bool canComputeMinMaxSrcId = dParam.canComputeMinMaxSrcId;
            bool canComputeIsTransfered = dParam.canComputeIsTransfered;
            bool canComputeWhereOfMultisearch = dParam.canComputeWhereOfMultisearch;
            bool canComputeSrcId = dParam.canComputeSrcId;
            bool canComputeUserSrcId = dParam.canComputeUserSrcId;


            List<UserCategoryModel> listUserCategory = dParam.listUserCategory;

            List<int> listProvinceId = dParam.listProvinceId;
            List<int> listDivisionId = dParam.listDivisionId;
            List<int> listDistrictId = dParam.listDistrictId;
            List<int> listTehsilId = dParam.listTehsilId;
            List<int> listUcId = dParam.listUcId;
            List<int> listWardId = dParam.listWardId;


            Dictionary<string, string> dictInjectableWhereClause = new Dictionary<string, string>();

            //string injectableWhereClause = "";
            //string hierarchyCheck = "";
            //string UserDesignationHierarchyCheck;
            //string FinalSQL = "";
            //string multicolumnSearchQuery;

            string paramKey = null;
            string tableParamKey = null;
            ListingParamsModelBase listingModelBase = null;
            listCategories = (listCategories==null || listCategories.Count==0) ? new List<int> { -1} : listCategories;

            //multicolumnSearchQuery = whereOfMultisearch + whereLogic;

            paramKey = "@param_";
            tableParamKey = "@tableParam_";

            #region HierarchyCheckComputation
            if (canComputeHierarchy)
            {
                switch ((Config.Hierarchy)hierarchyId)
                {
                    case Config.Hierarchy.Province:
                        dictInjectableWhereClause.Add("Where::regionCheck", DBHelper.InjectParameter(dictParam, " and complaints.Province_Id in (select id from {0})", tableParamKey, DBHelper.GetSqlParamTableAgainstList(tableParamKey+ dictParam.Count, listProvinceId)));
                        //hierarchyCheck = DBHelper.InjectParameter(dictParam, " and complaints.Province_Id in (select id from {0})", tableParamKey, provinceId);
                        //hierarchyCheck = string.Format(" and complaints.Province_Id in (select id from @{0})", paramKey);
                        //dictParam.Add(paramKey, provinceId);
                        break;

                    case Config.Hierarchy.Division:
                        dictInjectableWhereClause.Add("Where::regionCheck", DBHelper.InjectParameter(dictParam, " and complaints.Division_Id in (select id from {0})", tableParamKey, DBHelper.GetIdsDataTable(listDivisionId)));
                        //hierarchyCheck = string.Format(" and complaints.Division_Id in (select id from @{0})", paramKey);
                        //dictParam.Add(paramKey, divisionId);
                        break;

                    case Config.Hierarchy.District:
                        dictInjectableWhereClause.Add("Where::regionCheck", DBHelper.InjectParameter(dictParam, " and complaints.District_Id in (select id from {0})", tableParamKey, DBHelper.GetIdsDataTable(listDistrictId)));
                        //hierarchyCheck = string.Format(" and complaints.District_Id in (select id from @{0})", paramKey);
                        //dictParam.Add(paramKey, districtId);
                        break;

                    case Config.Hierarchy.Tehsil:
                        dictInjectableWhereClause.Add("Where::regionCheck", DBHelper.InjectParameter(dictParam, " and complaints.Tehsil_Id in (select id from {0})", tableParamKey, DBHelper.GetIdsDataTable(listTehsilId)));
                        //hierarchyCheck = string.Format(" and complaints.Tehsil_Id in (select id from @{0})", paramKey);
                        //dictParam.Add(paramKey, tehsilId);
                        break;

                    case Config.Hierarchy.UnionCouncil:
                        dictInjectableWhereClause.Add("Where::regionCheck", DBHelper.InjectParameter(dictParam, " and complaints.UnionCouncil_Id in (select id from {0})", tableParamKey, DBHelper.GetIdsDataTable(listUcId)));
                        //hierarchyCheck = string.Format(" and complaints.UnionCouncil_Id in (select id from @{0})", paramKey);
                        //dictParam.Add(paramKey, ucId);
                        break;

                    case Config.Hierarchy.Ward:
                        dictInjectableWhereClause.Add("Where::regionCheck", DBHelper.InjectParameter(dictParam, " and complaints.Ward_Id in (select id from {0})", tableParamKey, DBHelper.GetIdsDataTable(listUcId)));
                        //hierarchyCheck = string.Format(" and complaints.Ward_Id in (select id from @{0})", paramKey);
                        //dictParam.Add(paramKey, wardId);
                        break;
                }
            }

            #endregion

            DateTime currDate = DateTime.Now;

            //string complaintTypeCheck = "";

            //string complaintTypeSelection = "";
            //string ComplaintTypeHierarchyCheck = "";
            //string listingTypeCheck = "";
            //string CheckIfLowerHierarchyAndExistInSrcId;
            //string usercategoryCheck = "";
            //string regionHierarchyCheck = hierarchyCheck;


            if(canComputeComplaintType)
            {
                //complaintTypeCheck = string.Format(@"complaints.Complaint_Type={}");
                dictInjectableWhereClause.Add("Where::complaintTypeCheck", DBHelper.InjectParameter(dictParam, "and complaints.Complaint_Type={0}", paramKey, complaintType));
            }

            if (canComputeComplaintType && (Config.ComplaintType)complaintType == Config.ComplaintType.Complaint) // Complaint
            {
                //complaintTypeSelection =
                //    ",Complaint_Computed_Hierarchy_Id, Complaint_Computed_User_Hierarchy_Id, UserCategoryId1, UserCategoryId2, complaints.Complaint_Computed_Status as Complaint_Computed_Status, complaints.Complaint_Computed_Hierarchy  as Complaint_Computed_Hierarchy,  complaints.FollowupCount ";

                if ((Config.StakeholderComplaintListingType)listingType ==
                    Config.StakeholderComplaintListingType.AssignedToMe) //  Assigned To Me
                {
                    if (canComputeHierarchy)
                    {
                        if (userHierarchyId != null) //AND @UserDesignationHierarchyId<>0)
                        {
                            //ComplaintTypeSelection = ComplaintTypeSelection +
                            //                         ", complaints.Computed_Remaining_Time_To_Escalate as Computed_Remaining_Time_To_Escalate";

                            dictInjectableWhereClause.Add("Where::userHierarchyCheck", DBHelper.InjectParameter(dictParam, " and complaints.Complaint_Computed_User_Hierarchy_Id >= {0}", paramKey, userHierarchyId));
                            //paramKey = string.Format("@param_{0}", dictParam.Keys.Count);
                            //hierarchyCheck = hierarchyCheck + string.Format(" and complaints.Complaint_Computed_User_Hierarchy_Id >= {0}", userHierarchyId);
                            //dictParam.Add(paramKey, userHierarchyId);
                        }
                        if (!ignoreComputedHierarchyCheck)
                        {
                            dictInjectableWhereClause.Add("Where::hierarchyCheck", DBHelper.InjectParameter(dictParam, " AND complaints.Complaint_Computed_Hierarchy_Id <= {0}", paramKey, userHierarchyId));
                            //paramKey = string.Format("@param_{0}", dictParam.Keys.Count);
                            //listingTypeCheck = string.Format(" AND complaints.Complaint_Computed_Hierarchy_Id <= {0}", paramKey);
                            //dictParam.Add(paramKey, userHierarchyId);
                            //listingTypeCheck listingModelBase.UserHierarchyId;
                        }
                    }

                    //listingTypeCheck = listingTypeCheck + @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.TransferedStatus + "',',') X WHERE X.Item=complaints.IsTransferred)";
                    //listingTypeCheck = listingTypeCheck + @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.TransferedStatus + "',',') X WHERE X.Item=complaints.IsTransferred)";
                    //paramKey = "@tableParam_";
                    if (canComputeIsTransfered)
                    {
                        dictInjectableWhereClause.Add("Where::isTransferedCheck", DBHelper.InjectParameter(dictParam, " AND complaints.IsTransferred in (select id from {0})", tableParamKey, DBHelper.GetIdsDataTable(listTransferedStatus)));

                    }
                    //listingTypeCheck = DBHelper.InjectParameter(dictParam, " AND complaints.IsTransferred in (select id from {0})", tableParamKey, DBHelper.GetIdsDataTable(listTransferedStatus));

                    //ListingTypeCheck =
                    //    " AND complaints.Complaint_Computed_Hierarchy_Id <= " + listingModelBase.UserHierarchyId + @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + listingModelBase.TransferedStatus + "',',') X WHERE X.Item=complaints.IsTransferred)";
                    if (canComputeSrcId && checkIfExistInSrcId == 1 && !ignoreComputedHierarchyCheck)

                    {

                        dictInjectableWhereClause.Add("Where::canCheckSrcId",
                                            DBHelper.InjectParameter(dictParam, " complaints.SrcId1={0}", paramKey, userHierarchyId) 
                                            + DBHelper.InjectParameter(dictParam, " or complaints.SrcId2={0}", paramKey, userHierarchyId)
                                            + DBHelper.InjectParameter(dictParam, " or complaints.SrcId3={0}", paramKey, userHierarchyId)
                                            + DBHelper.InjectParameter(dictParam, " or complaints.SrcId4={0}", paramKey, userHierarchyId)
                                            + DBHelper.InjectParameter(dictParam, " or complaints.SrcId5={0}", paramKey, userHierarchyId)
                                            + DBHelper.InjectParameter(dictParam, " or complaints.SrcId6={0}", paramKey, userHierarchyId)
                                            + DBHelper.InjectParameter(dictParam, " or complaints.SrcId7={0}", paramKey, userHierarchyId)
                                            + DBHelper.InjectParameter(dictParam, " or complaints.SrcId8={0}", paramKey, userHierarchyId)
                                            + DBHelper.InjectParameter(dictParam, " or complaints.SrcId9={0}", paramKey, userHierarchyId)
                                            + DBHelper.InjectParameter(dictParam, " or complaints.SrcId10={0}", paramKey, userHierarchyId));


       //                 listingTypeCheck = listingTypeCheck + @"
							//and (complaints.SrcId1=" + listingModelBase.UserHierarchyId + @" OR complaints.SrcId2 = " + listingModelBase.UserHierarchyId + @"
							//OR complaints.SrcId3=" + listingModelBase.UserHierarchyId + @" OR complaints.SrcId4 = " + listingModelBase.UserHierarchyId + @"
							//OR complaints.SrcId5=" + listingModelBase.UserHierarchyId + @" OR complaints.SrcId6 = " + listingModelBase.UserHierarchyId + @"
							//OR complaints.SrcId7=" + listingModelBase.UserHierarchyId + @" OR complaints.SrcId8 = " + listingModelBase.UserHierarchyId + @"
							//OR complaints.SrcId9=" + listingModelBase.UserHierarchyId + @" OR complaints.SrcId10 = " + listingModelBase.UserHierarchyId + @")";
                    }
                    if (canComputeUserSrcId && canComputeUserHierarchy && canComputeUserSrcId)
                    {
                        dictInjectableWhereClause.Add("Where::canCheckUserSrcId",
                         string.Format("and ({0}=0", userHierarchyId)
                         +DBHelper.InjectParameter(dictParam, " OR complaints.UserSrcId1={0}", paramKey, userHierarchyId)
                         + DBHelper.InjectParameter(dictParam, " OR complaints.UserSrcId2={0}", paramKey, userHierarchyId)
                         + DBHelper.InjectParameter(dictParam, " OR complaints.UserSrcId3={0}", paramKey, userHierarchyId)
                         + DBHelper.InjectParameter(dictParam, " OR complaints.UserSrcId4={0}", paramKey, userHierarchyId)
                         + DBHelper.InjectParameter(dictParam, " OR complaints.UserSrcId5={0}", paramKey, userHierarchyId)
                         + DBHelper.InjectParameter(dictParam, " OR complaints.UserSrcId6={0}", paramKey, userHierarchyId)
                         + DBHelper.InjectParameter(dictParam, " OR complaints.UserSrcId7={0}", paramKey, userHierarchyId)
                         + DBHelper.InjectParameter(dictParam, " OR complaints.UserSrcId8={0}", paramKey, userHierarchyId)
                         + DBHelper.InjectParameter(dictParam, " OR complaints.UserSrcId9={0}", paramKey, userHierarchyId)
                         + DBHelper.InjectParameter(dictParam, " OR complaints.UserSrcId10={0}", paramKey, userHierarchyId)
                         + ")");


       //                 listingTypeCheck = listingTypeCheck + @"
							//and (" + listingModelBase.UserDesignationHierarchyId + @"=0 
							//OR complaints.UserSrcId1=" + listingModelBase.UserDesignationHierarchyId + @" OR complaints.UserSrcId2 = " + listingModelBase.UserDesignationHierarchyId + @"
							//OR complaints.UserSrcId3=" + listingModelBase.UserDesignationHierarchyId + @" OR complaints.UserSrcId4 = " + listingModelBase.UserDesignationHierarchyId + @"
							//OR complaints.UserSrcId5=" + listingModelBase.UserDesignationHierarchyId + @" OR complaints.UserSrcId6 = " + listingModelBase.UserDesignationHierarchyId + @"
							//OR complaints.UserSrcId7=" + listingModelBase.UserDesignationHierarchyId + @" OR complaints.UserSrcId8 = " + listingModelBase.UserDesignationHierarchyId + @"
							//OR complaints.UserSrcId9=" + listingModelBase.UserDesignationHierarchyId + @" OR complaints.UserSrcId10 = " + listingModelBase.UserDesignationHierarchyId + @")";
                    }
             

                }
                else if (canComputeListingType && listingType == 2) // Uptil my Hierarchy
                {
                    //complaintTypeSelection = complaintTypeSelection +
                    //                             ", complaints.Computed_Remaining_Time_To_Escalate as Computed_Remaining_Time_To_Escalate";

                    if (canComputeMinMaxSrcId)
                    {
                        dictInjectableWhereClause.Add("Where::canCheckMinMaxSrcId", DBHelper.InjectParameter(dictParam, " AND (complaints.MaxSrcId is null or complaints.MaxSrcId >= {0})", paramKey, userHierarchyId));
                    }
                    
                }

                if (canComputeStatus)
                {
                    dictInjectableWhereClause.Add("Where::canComputeStatus", DBHelper.InjectParameter(dictParam, " AND complaints.Complaint_Computed_Status_Id in (select id from {0})", tableParamKey, DBHelper.GetIdsDataTable(listComplaintStatuses)));
                }
            }
            else if (complaintType == 2 || complaintType == 3)
            // Suggestion and Inquiry
            {
                //complaintTypeSelection = "";
                //ComplaintTypeHierarchyCheck = "";

                //---------------------- End Category list Check ----------------

                //testing end
            }


            if (canComputeUserCategory)
            {
                string usercategoryCheck = "";
                //listingTypeCheck = @" AND (complaints.MaxSrcId is null or complaints.MaxSrcId >=" + listingModelBase.UserHierarchyId + @")";
                if (userCategoryId1 != null || userCategoryId2 != null)
                // new check
                {
                    if (userCategoryId1 != null)
                    {
                        usercategoryCheck += DBHelper.InjectParameter(dictParam, " and complaints.UserCategoryId1 = {0}", paramKey, userCategoryId1);
                        //usercategoryCheck = usercategoryCheck + " and complaints.UserCategoryId1 = " +
                        //                    listingModelBase.UserCategoryId1 + " ";
                    }
                    if (userCategoryId2 != null)
                    {
                        usercategoryCheck += DBHelper.InjectParameter(dictParam, " and complaints.UserCategoryId2 = {0}", paramKey, userCategoryId2);
                        //usercategoryCheck = usercategoryCheck + " and complaints.UserCategoryId2 = " +
                        //                    listingModelBase.UserCategoryId2 + " ";
                    }
                }

                //------------------- New User Category List Check -------------------
                //usercategoryCheck = "";
                if (!UserCategoryModel.AreAllCategoriesNull(listUserCategory))
                {
                    int count = 0;
                    foreach (UserCategoryModel userCategory in listUserCategory)
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

                dictInjectableWhereClause.Add("Where::userCategoryCheck", usercategoryCheck);
            }

            foreach(KeyValuePair<string,string> keyVal in dictInjectableWhereClause)
            {
                query += keyVal.Value;
            }
            query += dParam.whereToInject;

            //Dictionary<string, object> dictParams = new Dictionary<string, object>();
            //dictParams.Add("@OrderByColumnName", listingModelBase.OrderByColumnName.ToDbObj());
            //dictParams.Add("@OrderByDirection", listingModelBase.OrderByDirection.ToDbObj());
            //dictParams.Add("@InnerJoinLogic", listingModelBase.InnerJoinLogic.ToDbObj());
            //dictParams.Add("@ComplaintType", listingModelBase.ComplaintType.ToDbObj());
            //dictParams.Add("@Campaign", listingModelBase.Campaign.ToDbObj());
            //dictParams.Add("@From", listingModelBase.From.ToDbObj());
            //dictParams.Add("@To", listingModelBase.To.ToDbObj());
            //dictParams.Add("@Category", listingModelBase.Category.ToDbObj());
            //dictParams.Add("@usercategoryCheck", usercategoryCheck.ToDbObj());
            //dictParams.Add("@ComplaintTypeHierarchyCheck", ComplaintTypeHierarchyCheck);
            //dictParams.Add("@MulticolumnSearchQuery", multicolumnSearchQuery);
            //dictParams.Add("@HierarchyCheck", hierarchyCheck);
            //dictParams.Add("@StartRow", listingModelBase.StartRow);
            //dictParams.Add("@EndRow", listingModelBase.EndRow);

            //////----------  new columns ----------/////
            //dictParams.Add("@SpType", listingModelBase.SpType);
            //dictParams.Add("@SelectionFields", listingModelBase.SelectionFields);
            //dictParams.Add("@ComplaintTypeSelection", complaintTypeSelection.ToDbObj());
            //dictParams.Add("@UserId", listingModelBase.UserId.ToDbObj());
            //dictParams.Add("@GroupByLogic", listingModelBase.GroupByLogic.ToDbObj());
            //dictParams.Add("@WhereLogic", listingModelBase.WhereLogic.ToDbObj());


            dToRet.queryWhereClause = query;
            return dToRet;
        }
        public static string GetQuery(Dictionary<string, object> param)
        {
            string FinalSQL = string.Empty;

            if (param["@SpType"].ToString() == "Listing")
            {
                FinalSQL = @"
					    SELECT * from (SELECT " + param["@SelectionFields"] +
                           @"complaints.Id as ComplaintId,(CAST(complaints.Compaign_Id AS VARCHAR(10))+'-'+CAST(complaints.Id AS NVARCHAR(10))) AS Id, complaints.Campaign_Name Campaign_Name,complaints.Person_Name Person_Name,complaints.Province_Id Province_Id,complaints.Province_Name Province_Name,complaints.Division_Id Division_Id,complaints.Division_Name Division_Name,complaints.District_Id District_Id, complaints.District_Name District_Name, complaints.Tehsil_Id Tehsil_Id,Ref_Complaint_Id, complaints.Tehsil_Name Tehsil_Name, complaints.UnionCouncil_Id UnionCouncil_Id,complaints.UnionCouncil_Name UnionCouncil_Name,
				    CONVERT(VARCHAR(10),complaints.Created_Date,120)+RIGHT(CONVERT(VARCHAR, complaints.Created_Date, 100), 7) Created_Date,complaints.Complaint_Category_Name Complaint_Category_Name " +
                           param["@ComplaintTypeSelection"] + @", count(*)  OVER() AS Total_Rows,
				    ROW_NUMBER() OVER (ORDER BY " + param["@OrderByColumnName"] + " " + param["@OrderByDirection"] +
                           @" ) AS RowNum
				    FROM pitb.Complaints complaints " + param["@InnerJoinLogic"] + @"

                    --inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				    WHERE complaints.Complaint_Type = " + param["@ComplaintType"] +
                           string.Format(" and complaints.Compaign_Id in ({0})", param["@Campaign"]) +
                    @"AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + param["@From"] + @"' AND '" +
                           param["@To"] + @"' ) " +
                    string.Format(" and complaints.Complaint_Category in ({0}) ", param["@Category"])
                     + param["@usercategoryCheck"] + param["@ComplaintTypeHierarchyCheck"]
                           + param["@MulticolumnSearchQuery"] + @"
                    -- My code
			
				    --and complaints.Province_Id = @ProvinceId) as tbl
				     " + param["@HierarchyCheck"] + @") as tbl
			
				    WHERE tbl.RowNum BETWEEN @StartRow AND @EndRow 
                ";
            }

            if (param["@SpType"].ToString() == "UserComplaintsList")
            {
                FinalSQL = @"
					    SELECT * from (SELECT " + param["@SelectionFields"] +
                    " FROM pitb.Complaints complaints " + param["@InnerJoinLogic"] + @"

                    --inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				    WHERE complaints.Complaint_Type = " + param["@ComplaintType"] +
                           @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + param["@Campaign"] +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)
				    --AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + param["@From"] + @"' AND '" + param["@To"] + @"' ) 
				    AND EXISTS(SELECT 1 FROM dbo.SplitString('" + param["@Category"] +
                           @"',',') X WHERE X.Item=complaints.Complaint_Category)
				
				   " + param["@usercategoryCheck"] + param["@ComplaintTypeHierarchyCheck"]
                           + param["@MulticolumnSearchQuery"] + @"
                    -- My code
			
				    --and complaints.Province_Id = @ProvinceId) as tbl
				     " + param["@HierarchyCheck"] + @") as tbl";
            }

            if (param["@SpType"].ToString() == "MobileListingWithDateFilters")
            {
                FinalSQL = @"
					     SELECT * from (SELECT " + param["@SelectionFields"] +
                           @"complaints.Id as Complaint_Id,(CAST(complaints.Compaign_Id AS VARCHAR(10))+'-'+CAST(complaints.Id AS NVARCHAR(10))) AS Id, complaints.Campaign_Name Campaign_Name,complaints.Person_Name Person_Name, complaints.Person_Cnic,complaints.Person_Contact,
				complaints.District_Name District_Name, complaints.Tehsil_Name Tehsil_Name,complaints.UnionCouncil_Name UnionCouncil_Name, complaints.Computed_Remaining_Time_To_Escalate,complaints.StatusReopenedCount,
			 Created_Date,complaints.Complaint_Category_Name Complaint_Category_Name, complaints.Complaint_SubCategory_Name Complaint_SubCategory_Name,complaints.Complaint_Remarks,complaints.Complainant_Remark_Id,complaints.Complainant_Remark_Str " +
                           "" + @", count(*)  OVER() AS Total_Rows,
				    ROW_NUMBER() OVER (ORDER BY complaints.Created_Date DESC  " +
                           @" ) AS RowNum
				    FROM pitb.Complaints complaints " + param["@InnerJoinLogic"] + @"

                    --inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				    WHERE complaints.Complaint_Type = " + param["@ComplaintType"] +
                           @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + param["@Campaign"] +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)
				    AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + param["@From"] + @"' AND '" +
                           param["@To"] + @"' ) 
				    AND EXISTS(SELECT 1 FROM dbo.SplitString('" + param["@Category"] +
                           @"',',') X WHERE X.Item=complaints.Complaint_Category)
				
				   " + param["@usercategoryCheck"] + param["@ComplaintTypeHierarchyCheck"]
                           + param["@MulticolumnSearchQuery"] + @"
                    -- My code
			
				    --and complaints.Province_Id = @ProvinceId) as tbl
				     " + param["@HierarchyCheck"] + @") as tbl
			
				    WHERE tbl.RowNum BETWEEN " + param["@StartRow"] + @" AND " + param["@EndRow"] + @"

                ";
            }

            else if (param["@SpType"].ToString() == "ListingExpiring")
            {
                FinalSQL = @"
					    SELECT * from (SELECT " + param["@SelectionFields"] +
                           @"complaints.Id as ComplaintId,(CAST(complaints.Compaign_Id AS VARCHAR(10))+'-'+CAST(complaints.Id AS NVARCHAR(10))) AS Id, complaints.Campaign_Name Campaign_Name,complaints.Person_Name Person_Name, complaints.District_Name District_Name, complaints.Tehsil_Name Tehsil_Name,complaints.UnionCouncil_Name UnionCouncil_Name,
				    CONVERT(VARCHAR(10),complaints.Created_Date,120) Created_Date,complaints.Complaint_Category_Name Complaint_Category_Name " +
                           param["@ComplaintTypeSelection"] + @", count(*)  OVER() AS Total_Rows,
				    ROW_NUMBER() OVER (ORDER BY " + param["@OrderByColumnName"] + " " + param["@OrderByDirection"] +
                           @" ) AS RowNum
				    FROM pitb.Complaints complaints " + param["@InnerJoinLogic"] + @"

                    --inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				    WHERE complaints.Complaint_Type = " + param["@ComplaintType"] +
                           @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + param["@Campaign"] +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)
				    AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + param["@From"] + @"' AND '" +
                           param["@To"] + @"' ) 
				    AND EXISTS(SELECT 1 FROM dbo.SplitString('" + param["@Category"] +
                           @"',',') X WHERE X.Item=complaints.Complaint_Category) 
                    AND complaints.Computed_Remaining_Time_Percentage > 0 and complaints.Computed_Remaining_Time_Percentage <= 30  
				
				   " + param["@usercategoryCheck"] + param["@ComplaintTypeHierarchyCheck"]
                           + param["@MulticolumnSearchQuery"] + @"
                    -- My code
			
				    --and complaints.Province_Id = @ProvinceId) as tbl
				     " + param["@HierarchyCheck"] + @") as tbl
			
				    WHERE tbl.RowNum BETWEEN " + param["@StartRow"] + @" AND " + param["@EndRow"] + @"

                ";
            }




            if (param["@SpType"].ToString() == "ComplaintsAssignedToUser")
            {
                FinalSQL = @"
					    SELECT  complaints.Id as ComplaintId
				    FROM pitb.Complaints complaints " + param["@InnerJoinLogic"] + @"

                    --inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				    WHERE complaints.Complaint_Type = " + param["@ComplaintType"] +
                           @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + param["@Campaign"] +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)
				    AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + param["@From"] + @"' AND '" +
                           param["@To"] + @"' ) 
				    AND EXISTS(SELECT 1 FROM dbo.SplitString('" + param["@Category"] +
                           @"',',') X WHERE X.Item=complaints.Complaint_Category)
				
				   " + param["@usercategoryCheck"] + param["@ComplaintTypeHierarchyCheck"]
                           + param["@MulticolumnSearchQuery"] + @"
                    -- My code
			
				    --and complaints.Province_Id = @ProvinceId) as tbl
				     " + param["@HierarchyCheck"] + @"
                   ORDER BY complaints.Id 

				   -- WHERE tbl.RowNum BETWEEN " + param["@StartRow"] + @" AND " + param["@EndRow"] + @"

                ";
            }

            else if (param["@SpType"].ToString() == "ExcelReport")
            {
                //FinalSQL = QueryHelper.GetFinalQuery("ComplaintsListingAssignedToMe", Config.ConfigType.Query, dictParams);

                //------if tag query exists------

                //------ end tag exit-------------



                FinalSQL = @"
				SELECT * FROM(
				SELECT 
					" + param["@SelectionFields"] + @"

                     FROM pitb.Complaints complaints " + param["@InnerJoinLogic"] + @"


                    WHERE 
							complaints.Complaint_Type = " + param["@ComplaintType"] + @"

                            AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + param["@From"] + @"' AND '" + param["@To"] +
                           @"' )
							AND EXISTS(SELECT 1 FROM dbo.SplitString('" + param["@Campaign"] +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)
							AND EXISTS(SELECT 1 FROM dbo.SplitString('" + param["@Category"] +
                           @"',',') X WHERE X.Item=complaints.Complaint_Category)
							" + param["@ComplaintTypeHierarchyCheck"]
                           + param["@MulticolumnSearchQuery"]
                           + param["@HierarchyCheck"] + @"
							
							)Data ORDER BY Data.[Created Date] DESC";
            }

            else if (param["@SpType"].ToString() == "DashboardLabelsStausWise")
            {
                FinalSQL = @"
					
				select * from (SELECT statuses.Id AS Id, statuses.Status AS Name, COUNT(1) AS Count --, (CASE WHEN COUNT(1) = 1 THEN 0 else COUNT(1) END) AS Count 
				FROM PITB.Statuses statuses inner JOIN pitb.Complaints complaints ON complaints.Complaint_Computed_Status_Id = statuses.Id
			    " + param["@InnerJoinLogic"] + @"
                --inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				WHERE complaints.Complaint_Type = " + param["@ComplaintType"] + @" and EXISTS(SELECT 1 FROM dbo.SplitString('" +
                           param["@Campaign"] + @"',',') X WHERE X.Item=complaints.Compaign_Id)
				AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + param["@From"] + @"' AND '" + param["@To"] + @"' ) 
				AND EXISTS(SELECT 1 FROM dbo.SplitString('" + param["@Category"] +
                           @"',',') X WHERE X.Item=complaints.Complaint_Category)
				" + param["@usercategoryCheck"] + param["@ComplaintTypeHierarchyCheck"]
                           + param["@MulticolumnSearchQuery"]
                           + param["@HierarchyCheck"] + @"

                 GROUP BY statuses.Id, statuses.Status
				 ) as tbl
				 
		        ";
            }

            else if (param["@SpType"].ToString() == "DashboardLabelsComplaintSrc")
            {
                FinalSQL = @"
					
				select * from (SELECT complaints.ComplaintSrc AS Id, COUNT(1) AS Count --, (CASE WHEN COUNT(1) = 1 THEN 0 else COUNT(1) END) AS Count 
				FROM pitb.Complaints complaints " + param["@InnerJoinLogic"] + @"

                --inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				WHERE complaints.Complaint_Type = " + param["@ComplaintType"] + @" and EXISTS(SELECT 1 FROM dbo.SplitString('" +
                           param["@Campaign"] + @"',',') X WHERE X.Item=complaints.Compaign_Id)
				AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + param["@From"] + @"' AND '" + param["@To"] +
                           @"' ) 
				AND EXISTS(SELECT 1 FROM dbo.SplitString('" + param["@Category"] +
                           @"',',') X WHERE X.Item=complaints.Complaint_Category)
				
				" + param["@usercategoryCheck"] + param["@ComplaintTypeHierarchyCheck"]
                           + param["@MulticolumnSearchQuery"]
                           + param["@HierarchyCheck"] + @"

                 GROUP BY complaints.ComplaintSrc
				 ) as tbl
				 
		        ";
            }

            else if (param["@SpType"].ToString() == "StatusWiseUserComplaints")
            {
                FinalSQL = @"
					    --SELECT " + param["@SelectionFields"] +
                           @"--complaints.Id as ComplaintId,(CAST(complaints.Compaign_Id AS VARCHAR(10))+'-'+CAST(complaints.Id AS NVARCHAR(10))) AS Id, complaints.Campaign_Name Campaign_Name,complaints.Person_Name Person_Name, complaints.District_Name District_Name, complaints.Tehsil_Name Tehsil_Name,complaints.UnionCouncil_Name UnionCouncil_Name,
				    --CONVERT(VARCHAR(10),complaints.Created_Date,120) Created_Date,complaints.Complaint_Category_Name Complaint_Category_Name
                    SELECT " + param["@UserId"] + @" as User_Id, complaints.Complaint_Computed_Status_Id, complaints.Complaint_Computed_Status, COUNT(*) Count 
				    FROM pitb.Complaints complaints  " + param["@InnerJoinLogic"] + @"

                    --inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				    WHERE complaints.Complaint_Type = " + param["@ComplaintType"] +
                           @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + param["@Campaign"] +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)
				    AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + param["@From"] + @"' AND '" +
                           param["@To"] + @"' ) 
				    AND EXISTS(SELECT 1 FROM dbo.SplitString('" + param["@Category"] +
                           @"',',') X WHERE X.Item=complaints.Complaint_Category)
				
				   " + param["@usercategoryCheck"] + param["@ComplaintTypeHierarchyCheck"]
                           + param["@MulticolumnSearchQuery"] + @"
                    -- My code
			
				    --and complaints.Province_Id = @ProvinceId) as tbl
				     " + param["@HierarchyCheck"] + @"
                    GROUP BY complaints.Complaint_Computed_Status_Id, complaints.Complaint_Computed_Status
		        ";
            }

            else if (param["@SpType"].ToString() == "CategoryWiseStatusComplaintCounts")
            {
                FinalSQL = @"

                    SELECT " + param["@SelectionFields"] + @", complaints.Complaint_Computed_Status_Id, complaints.Complaint_Computed_Status, COUNT(*) Count 
				    FROM pitb.Complaints complaints  " + param["@InnerJoinLogic"] + @"

                    --inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				    WHERE complaints.Complaint_Type = " + param["@ComplaintType"] +
                           @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + param["@Campaign"] +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)
				    AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + param["@From"] + @"' AND '" +
                           param["@To"] + @"' ) 
				   " + param["@usercategoryCheck"] + param["@ComplaintTypeHierarchyCheck"]
                           + param["@MulticolumnSearchQuery"] + @"
                    -- My code
			
				    --and complaints.Province_Id = @ProvinceId) as tbl
				     " + param["@HierarchyCheck"] +
                      " Group by " + param["@GroupByLogic"] + "";
            }

            else if (param["@SpType"].ToString() == "OverDueComplaintsSummary")
            {
                FinalSQL = @"

                    SELECT " + param["@SelectionFields"] + @"

                    FROM pitb.Complaints complaints  " + param["@InnerJoinLogic"] + @"

                    --inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				    WHERE complaints.Complaint_Type = " + param["@ComplaintType"] +
                           @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + param["@Campaign"] +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)
				    AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + param["@From"] + @"' AND '" +
                           param["@To"] + @"' ) 
				   " + param["@usercategoryCheck"] + param["@ComplaintTypeHierarchyCheck"]
                           + param["@MulticolumnSearchQuery"] + @"
                    -- My code
			
				    --and complaints.Province_Id = @ProvinceId) as tbl
				     " + param["@HierarchyCheck"] + "";
            }

            else if (param["@SpType"].ToString() == "RegionStatusWiseSummary")
            {
                FinalSQL = @"

                    SELECT " + param["@SelectionFields"] + @"

                    FROM pitb.Complaints complaints  " + param["@InnerJoinLogic"] + @"

                    --inner join pitb.Statuses b on complaints.Complaint_Computed_Status_Id = b.id

				    WHERE complaints.Complaint_Type = " + param["@ComplaintType"] +
                           @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + param["@Campaign"] +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)
				    AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + param["@From"] + @"' AND '" +
                           param["@To"] + @"' ) 
				   " + param["@usercategoryCheck"] + param["@ComplaintTypeHierarchyCheck"]
                           + param["@MulticolumnSearchQuery"] + @"
                    -- My code
			
				    --and complaints.Province_Id = @ProvinceId) as tbl
				     " + param["@HierarchyCheck"] +
                    " Group by " + param["@GroupByLogic"] + "";

            }

            else if (param["@SpType"].ToString() == "DynamicFieldsWiseCounts")
            {
                FinalSQL = @"
					    SELECT " + param["@SelectionFields"] + @", dynamicFields.CategoryTypeId,dynamicFields.FieldValue, COUNT(1) Count
                        --complaints.id,dynamicFields.CategoryTypeId , dynamicFields.FieldName,dynamicFields.FieldValue
                        FROM pitb.Complaints complaints INNER JOIN pitb.Dynamic_ComplaintFields dynamicFields
                        ON complaints.Id=dynamicFields.ComplaintId 
                        WHERE dynamicFields.ControlId = " + param["@DynamicFieldsControlId"] + @" AND complaints.Complaint_Type = 1 
                        AND (CONVERT(DATE,complaints.Created_Date,120) BETWEEN '" + param["@From"] + @"' AND '" +
                           param["@To"] + @"' ) " + @" and EXISTS(SELECT 1 FROM dbo.SplitString('" + param["@Campaign"] +
                           @"',',') X WHERE X.Item=complaints.Compaign_Id)" + param["@WhereLogic"] + @"
                        GROUP BY " + param["@GroupByLogic"] + ", dynamicFields.CategoryTypeId,dynamicFields.FieldValue";
            }

            return FinalSQL;
        }
    }
}