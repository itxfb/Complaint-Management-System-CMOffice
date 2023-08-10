using System.Linq;
using System.Web.UI;
using PITB.CMS.Helper.Database;
using PITB.CMS.Helper;
using PITB.CMS.Models.Custom;

namespace PITB.CMS.Models.DB
{
    using Microsoft.SqlServer.Server;
    using PITB.CMS.Helper.Database;
    using PITB.CMS.Helper.Extensions;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Dynamic;

    [Table("PITB.AssignmentMatrix")]
    public partial class DbAssignmentMatrix
    {
        [Column(TypeName = "numeric")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public decimal Id { get; set; }

        public int? CampaignId { get; set; }

        public int? CategoryType { get; set; }

        public int? CategoryId { get; set; }

        public int? CategoryDep1 { get; set; }

        public int? CategoryDep2 { get; set; }

        public int? HierarchyLevel { get; set; }

        public int? HierarchyId { get; set; }

        public int? FromSourceId { get; set; }

        public int? ToSourceId { get; set; }

        public int? ToUserSourceId { get; set; }

        public int? LevelId { get; set; }
        public double RetainingHours { get; set; }

        public string TagStr { get; set; }

        public bool? IsActive { get; set; }

        //[NotMapped]
        //public string[] ArrTag { get; set; }

        //[NotMapped]
        //public Dictionary<string, string> DictTag { get; set; }

        #region Helpers
        public static List<DbAssignmentMatrix> GetByCampaignIdAndCategoryId(params object[]objArr /*int campId, int catId, int subCatId, int? categoryDep1 = null, int? categoryDep2 = null, List<Pair<int?,int?>> listHierarchy = null*//*, int? hierarchyLevel=null, int? hierarchyId=null*/)
            {
                objArr = objArr.ToNullableObjsArr(7);

                int campId = (int)objArr[0];
                int catId = (int)objArr[1];
                int subCatId = (int)objArr[2];
                int? categoryDep1 = (int?)objArr[3];
                int? categoryDep2 = (int?)objArr[4];
                List<Pair<int?, int?>> listHierarchy = (List < Pair<int?, int?>>) objArr[5];
                string assignmentTag = (string)objArr[6];



                if (listHierarchy == null)
                {
                    listHierarchy = new List<Pair<int?, int?>>{new Pair<int?, int?>(null,null)};
                }

                // Start Custom Code
                try
                {
                    DbAssignmentMatrix matrixToComp = new DbAssignmentMatrix();
                    matrixToComp.CampaignId = campId;
                    matrixToComp.CategoryType = (int) Config.CategoryType.Sub;
                    matrixToComp.CategoryId = subCatId;
                    matrixToComp.CategoryDep1 = categoryDep1;
                    matrixToComp.CategoryDep2 = categoryDep2;
                    matrixToComp.HierarchyLevel = listHierarchy[0].Item1;
                    matrixToComp.HierarchyId = listHierarchy[0].Item2;

                    using (var db = new DBContextHelperLinq())
                    {
                        List<DbAssignmentMatrix> listAssignMatrix = null, listTempAssignmentMatrix = null;
                        //List<DbAssignmentMatrix> listAssignMatrixTemp = null;
                        //List<DbAssignmentMatrix> listAssignMatrixTemp2 = null;
                        // For Sub Category
                        listAssignMatrix = db.DbAssignmentMatrix.AsNoTracking().Where(n => n.CampaignId == campId && n.IsActive==true).OrderBy(n => n.LevelId).ToList();

                        if (assignmentTag != null)
                        {
                            listTempAssignmentMatrix = GetListAssignmentMatrixByTag(listAssignMatrix, assignmentTag);
                            if (listTempAssignmentMatrix.Count > 0)
                            {
                                return listTempAssignmentMatrix;
                            }
                        }

                        bool hasCat = false, hasCatDep = false, hasHierarchy = false;

                        //hasCat = (listAssignMatrix.Where(n => n.CategoryType != null).FirstOrDefault() != null);
                        //hasCatDep = (listAssignMatrix.Where(n => n.CategoryDep1 != null || n.CategoryDep2 != null).FirstOrDefault() !=null);
                        //hasHierarchy = (listAssignMatrix.Where(n => n.HierarchyLevel != null).FirstOrDefault() !=null);

                        //listTempAssignmentMatrix = listAssignMatrix.ToList();
                        listTempAssignmentMatrix = listAssignMatrix.Where(n => n.CategoryType != null && n.IsActive == true).ToList();
                        
                        //Starting Alternate Logic
                        
                        //If subcat exist
                        listTempAssignmentMatrix = listAssignMatrix.GetMatchedFieldResults(matrixToComp,
                            "CategoryType,CategoryId");

                        

                        // If subcategory does not exist
                        if (listTempAssignmentMatrix.Count == 0)
                        {
                            //If category exist
                            matrixToComp.CategoryType = (int)Config.CategoryType.Main;
                            matrixToComp.CategoryId = catId;
                            listTempAssignmentMatrix = listAssignMatrix.GetMatchedFieldResults(matrixToComp,
                                "CategoryType,CategoryId");

                            
                            // if category doesnot exists : Get Default Assignment Matrix
                            if (listTempAssignmentMatrix.Count == 0)
                            { 
                                //matrixToComp.CategoryType = null;
                                //matrixToComp.CategoryId = null;
                                listTempAssignmentMatrix = GetListAssignmentMatrix(listAssignMatrix, matrixToComp,
                                    "CategoryType,CategoryId,TagStr");

                                listTempAssignmentMatrix = GetListAssignmentMatrix(listTempAssignmentMatrix, matrixToComp,
                                    "CategoryDep1,CategoryDep2,TagStr");

                                listTempAssignmentMatrix = GetListAssignmentMatrix(listTempAssignmentMatrix, matrixToComp,
                                    "HierarchyLevel,HierarchyId,TagStr", listHierarchy);
                            }
                            else // if category exists further filter data from listTempAssignmentMatrix
                            {
                                listTempAssignmentMatrix = listTempAssignmentMatrix.GetMatchedFieldResults(matrixToComp,
                               "CategoryDep1,CategoryDep2,TagStr");

                                listTempAssignmentMatrix = GetListAssignmentMatrix(listTempAssignmentMatrix,matrixToComp,
                               "HierarchyLevel,HierarchyId,TagStr", listHierarchy);
                            }
                        }
                        else // if subcategory exist
                        {

                            listTempAssignmentMatrix = GetListAssignmentMatrix(listTempAssignmentMatrix, matrixToComp,
                               "CategoryDep1,CategoryDep2,TagStr");

                            listTempAssignmentMatrix = GetListAssignmentMatrix(listTempAssignmentMatrix, matrixToComp,
                               "HierarchyLevel,HierarchyId,TagStr", listHierarchy);

                            //listTempAssignmentMatrix = GetListAssignmentMatrix(listTempAssignmentMatrix, matrixToComp,
                            //   "HierarchyLevel,HierarchyId");
                          
                        }

                        // ending alternate logic
                        /*
                        if (listTempAssignmentMatrix.Count > 0)
                        {
                            // check if subcategory is present
                            listTempAssignmentMatrix = listAssignMatrix.Where(n => n.CategoryId == subCatId && n.CategoryType == (int)Config.CategoryType.Sub).OrderBy(n => n.LevelId).ToList();
                            
                            // if subcategory not present then check category
                            if (listTempAssignmentMatrix.Count == 0)
                            {
                                listTempAssignmentMatrix =
                                    listAssignMatrix.Where(
                                        n => n.CategoryId == catId && n.CategoryType == (int) Config.CategoryType.Main)
                                        .OrderBy(n => n.LevelId)
                                        .ToList();
                            }

                            // if there is no category or subcategory then check for category department
                            if (listTempAssignmentMatrix.Count == 0) // if no category wise partition then take the orignal one
                            {
                                listTempAssignmentMatrix = listAssignMatrix.Where(n => n.CategoryType == null && (n.CategoryDep1 != categoryDep1 || n.CategoryDep2 != categoryDep2)).ToList();
                            }
                        }

                        if (hasCatDep)
                        {
                            if (listTempAssignmentMatrix.Count == 0) // if no category wise partition then take the orignal one
                            {
                                listTempAssignmentMatrix = listAssignMatrix.Where(n => n.CategoryDep1 != categoryDep1 || n.CategoryDep2 != categoryDep2).ToList();
                            }
                        }
                        */
                        return listTempAssignmentMatrix;
                    }
                }
                catch (Exception)
                {

                    throw;
                }
                // End Custom Code
                
                //try
                //{
                //    using (var db = new DBContextHelperLinq())
                //    {
                //        List<DbAssignmentMatrix> listAssignMatrix = null;
                //        List<DbAssignmentMatrix> listAssignMatrixTemp = null;
                //        List<DbAssignmentMatrix> listAssignMatrixTemp2 = null;
                //        // For Sub Category
                //        listAssignMatrix = db.DbAssignmentMatrix.AsNoTracking().Where(n => n.CampaignId == campId && n.CategoryId == subCatId && n.CategoryType == (int)Config.CategoryType.Sub).OrderBy(n => n.LevelId).ToList();
                        
                //        if (listAssignMatrix.Count == 0)// if sub category is not find in assignment matrix
                //        {
                //            listAssignMatrix = db.DbAssignmentMatrix.AsNoTracking().Where(n => n.CampaignId == campId && n.CategoryId == catId && n.CategoryType == (int)Config.CategoryType.Main).OrderBy(n => n.LevelId).ToList();
                //        }
                //        if (listAssignMatrix.Count == 0)// if category is not find in assignment matrix
                //        {
                //            listAssignMatrix = db.DbAssignmentMatrix.AsNoTracking().Where(n => n.CampaignId == campId && n.CategoryId==null && n.CategoryDep1==null && n.CategoryDep2==null).OrderBy(n => n.LevelId).ToList();
                //        }

                //        listAssignMatrixTemp = listAssignMatrix.Where(n => n.CategoryDep1 == categoryDep1) // if catDepExist
                //            .ToList();

                //        listAssignMatrixTemp2 =
                //            listAssignMatrix.Where(n => n.CategoryDep1 != null || n.CategoryDep2 != null).ToList(); // 

                //        if (listAssignMatrixTemp.Count>0) //  if categoryDep exist in any of them
                //        {
                //            listAssignMatrix =
                //                listAssignMatrixTemp.Where(
                //                    n => n.CategoryDep1 == categoryDep1).OrderBy(n => n.LevelId).ToList();
                //        }
                //        else if (listAssignMatrixTemp.Count == 0 && listAssignMatrixTemp2.Count>0) // if curr catDep doesnt exist but category still exist
                //        {
                //            listAssignMatrix = listAssignMatrix.Except(listAssignMatrixTemp2).ToList();
                //        }
                        

                //        return listAssignMatrix;
                //    }
                //}
                //catch (Exception)
                //{

                //    throw;
                //}
                
            }

            public static List<DbAssignmentMatrix> GetListAssignmentMatrixByTag (List<DbAssignmentMatrix> listAssignMatrix, string assignmentTag)
            {
                List<DbAssignmentMatrix> listAssignmentMatrixToRet = null;
                List<dynamic> listDynamic = new List<dynamic>();
                dynamic d = null;
                //List<List<DbAssignmentMatrix>> list

                string[] arrParentAssigmentMatrixTag = assignmentTag.Split(new string[] { "___" }, StringSplitOptions.None);//.ToList() ;
                string[] arrChildAssignmentMatrixTag = null;

                var groupList = listAssignMatrix.Where(n => n.TagStr != null).GroupBy(n => n.TagStr).ToArray();
                
                foreach (var group in groupList)
                {
                    d = new ExpandoObject(); 
                    d.listAssignmentMatrix = new List<DbAssignmentMatrix>();
                    foreach (DbAssignmentMatrix dbAssignmentMatrix in listAssignMatrix.Where(n => n.TagStr == group.Key).ToArray())
                    {
                        arrChildAssignmentMatrixTag = dbAssignmentMatrix.TagStr.Split(new string[] { "___" }, StringSplitOptions.None);
                        if (arrChildAssignmentMatrixTag.Where(n => arrParentAssigmentMatrixTag.Contains(n)).Count() > 0)
                        {
                            d.listAssignmentMatrix.Add(dbAssignmentMatrix);
                        }
                    }
                    listDynamic.Add(d);
                }
                d = listDynamic.OrderByDescending(x => x.listAssignmentMatrix.Count).FirstOrDefault();
                if(d!=null)
                {
                    listAssignmentMatrixToRet = d.listAssignmentMatrix;
                }
               
                return listAssignmentMatrixToRet;




            //List<DbAssignmentMatrix> listAssignmentMatrixToRet = new List<DbAssignmentMatrix>();
            //Dictionary<string,string> dictAssignmentTag = Utility.ConvertCollonFormatToDict(assignmentTag);

            //var groupList = listAssignMatrix.Where(n=>n.TagStr!=null).GroupBy(n => n.TagStr).ToList();

            //foreach(var group in groupList)
            //{
            //    foreach (DbAssignmentMatrix dbAssignmentMatrix in listAssignMatrix.Where(n => n.TagStr == group.Key).ToList())
            //    {
            //        dbAssignmentMatrix.DictTag = Utility.ConvertCollonFormatToDict(dbAssignmentMatrix.TagStr);
            //        if (dbAssignmentMatrix.DictTag.Where(n => dictAssignmentTag.Keys.Contains(n.Key)).Count() > 0)
            //        {
            //            listAssignmentMatrixToRet.Add(dbAssignmentMatrix);
            //        }
            //    }
            //}
            //return listAssignmentMatrixToRet;
        }

            private static List<DbAssignmentMatrix> GetListAssignmentMatrix (List<DbAssignmentMatrix> listAssignMatrix, DbAssignmentMatrix dbAssignmentMatrix, string fields)
            {
                List<DbAssignmentMatrix> listTempAssignmentMatrix = listAssignMatrix.GetMatchedFieldResults(dbAssignmentMatrix, fields);
                if (listTempAssignmentMatrix.Count == 0)
                {
                    List<string> listStr = fields.Split(',').ToList();
                    foreach (string str in listStr)
                    {
                        Utility.SetPropertyThroughReflection(dbAssignmentMatrix, str, (int?) null);
                    }

                    listTempAssignmentMatrix = listAssignMatrix.GetMatchedFieldResults(dbAssignmentMatrix, fields);
                }

                return listTempAssignmentMatrix;
            }


            private static List<DbAssignmentMatrix> GetListAssignmentMatrix(List<DbAssignmentMatrix> listAssignMatrix, DbAssignmentMatrix dbAssignmentMatrix, string fields,List<Pair<int?,int?>> listHierarchy )
            {
                List<DbAssignmentMatrix> listTempAssignmentMatrix = null;
                bool hasHierarchyResult = false;
                for (int i = 0; i < listHierarchy.Count && !hasHierarchyResult; i++)
                {
                    dbAssignmentMatrix.HierarchyLevel = listHierarchy[i].Item1;
                    dbAssignmentMatrix.HierarchyId = listHierarchy[i].Item2;
                    
                    listTempAssignmentMatrix = listAssignMatrix.GetMatchedFieldResults(dbAssignmentMatrix, fields);
                    if (listTempAssignmentMatrix.Count > 0)
                    {
                        hasHierarchyResult = true;
                    }
                }
                if (!hasHierarchyResult)
                {
                    List<string> listStr = fields.Split(',').ToList();
                    foreach (string str in listStr)
                    {
                        Utility.SetPropertyThroughReflection(dbAssignmentMatrix, str, (int?)null);
                    }

                    listTempAssignmentMatrix = listAssignMatrix.GetMatchedFieldResults(dbAssignmentMatrix, fields);
                }

                return listTempAssignmentMatrix;
            }
        #endregion
    }
}
