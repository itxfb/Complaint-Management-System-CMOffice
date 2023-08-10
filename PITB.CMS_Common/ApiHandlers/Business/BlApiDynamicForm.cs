using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PITB.CMS_Common.Handler.Business;
using PITB.CMS_Common.Models;
using PITB.CMS_Common.Models.Custom.CustomForm;
using PITB.CMS_Common.Modules.Api.Response;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PITB.CMS_Common.ApiHandlers.Business
{
    public class BlApiDynamicForm
    {
        public static dynamic AppStart(dynamic d/*, HttpRequest request*/)
        {
            #region variables
            dynamic response = new ExpandoObject();
            CustomForm.Post customForm = d;
            string formToLoad = string.Empty;
            IEnumerable<IGrouping<string, DbDynamicFormControls>> formGroups;
            dynamic dataObj;
            int userId = 0;
            DbUsers dbUser = null;
            bool tokenExists = false;
            List<object> listCampaignIds = null;
            List<object> listofStatues = null;
            List<object> listofStatuesAll = null;
            List<dynamic> listofStatusesMeAndAll = null;
            List<DbStatus> listDbStatusesMultiSel = null;
            //bool onlyChiniotDistrict = false;
            List<DbDistrict> listDbDistTemp = null;
            //List<DbDistrict> listDbDistrict = null;
            //List<DbTehsil> listDbTehsil = null;

            #endregion


            #region get forms
            string dataJson = customForm.GetElementValue("data");
            dataObj = Utility.DeserializeToDynamic(dataJson);

            if (!string.IsNullOrEmpty(dataObj.userToken))
            {
                dbUser = DbUsers.GetActiveUser((int)DbUsers.GetUserIdFromHashString(dataObj.userToken));
                if (dbUser.IsAllowed != null && dbUser.IsAllowed == false)
                {
                    response.data = null;
                    response.status = false;
                    response.message = "User Unauthorized";
                    return ApiResponseHandlerMobile.SetUnauthorizedError(response);
                }
            }

            if (dataJson.ToLower().Contains("usertoken"))
            {
                try
                {
                    tokenExists = true;
                    if (!string.IsNullOrEmpty(dataObj.userToken))
                    {
                        userId = DbUsers.GetUserIdFromHashString(dataObj.userToken);
                        dbUser = DbUsers.GetActiveUser(userId);
                    }
                }
                catch (Exception ex)
                {
                    response = ApiResponseHandlerMobile.SetNoDataFound(ApiResponseHandlerMobile.SetData(null), "No Record Found");
                    return response;
                }
            }


            if (dbUser != null && (int)dbUser.Role_Id == 3) // dcchiniot resolver 
            {
                formToLoad = "Listing"; //only load this form
                formGroups = DbDynamicFormControls.GetByCampaignId(99).OrderBy(o => o.FormPriority).ThenBy(o => o.Priority).GroupBy(g => g.TagId).Where(w => w.Key == formToLoad);

            }
            else if (userId != 0 && dbUser != null && (int)dbUser.Role_Id == 9 && tokenExists == true) // complaint add form
            {
                formToLoad = "PublicUserSignupForm"; //do not load this form
                formGroups = DbDynamicFormControls.GetByCampaignId(99).OrderBy(o => o.FormPriority).ThenBy(o => o.Priority).GroupBy(g => g.TagId).Where(w => w.Key != formToLoad);
                //onlyChiniotDistrict = true;
            }
            else // sign up form
            {
                formToLoad = "PublicUserSignupForm"; //load only this form
                formGroups = DbDynamicFormControls.GetByCampaignId(99).OrderBy(o => o.FormPriority).ThenBy(o => o.Priority).GroupBy(g => g.TagId).Where(w => w.Key == formToLoad);
                //onlyChiniotDistrict = false;
            }
            #endregion



            #region statues and campaign
            if (dbUser != null)
            {
                Dictionary<string, string> dictHeaders = customForm.DictHeaders;
                DbClientSystem dbClientSystem = DbClientSystem.GetBy(dictHeaders["SystemName"], dictHeaders["Username"], dictHeaders["Password"]);
                dynamic appData = JsonConvert.DeserializeObject<ExpandoObject>(dbClientSystem.AppData);
                listCampaignIds = appData.app.listCampaignId;
                if ((int)dbUser.Role_Id == 3)
                {
                    listofStatues = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(2, dbUser.User_Id,(int)Config.Permissions.StatusesForComplaintListing).FirstOrDefault().Permission_Value.Split(',').ToList().ConvertAll(x => (object)x);
                    listDbStatusesMultiSel = DbStatus.GetByStatusIds(listofStatues.ConvertAll(x => Convert.ToInt32(x)));
                    listofStatusesMeAndAll = listDbStatusesMultiSel.Select(n => new { id = n.Complaint_Status_Id, name = n.Status, parentId = (int)Config.StakeholderComplaintListingType.AssignedToMe }).Cast<object>().ToList();

                    listofStatuesAll = DbPermissionsAssignment.GetListOfPermissionsByTypeTypeIdAndPermissionId(2, dbUser.User_Id, (int)Config.Permissions.StatusesForComplaintListingAll).FirstOrDefault().Permission_Value.Split(',').ToList().ConvertAll(x => (object)x);
                    listDbStatusesMultiSel = DbStatus.GetByStatusIds(listofStatuesAll.ConvertAll(x => Convert.ToInt32(x)));
                    listofStatusesMeAndAll.AddRange( listDbStatusesMultiSel.Select(n => new { id = n.Complaint_Status_Id, name = n.Status, parentId = (int)Config.StakeholderComplaintListingType.UptilMyHierarchy }).Cast<object>().ToList());
                }
                else
                {
                    listofStatues = appData.app.listofStatuesAssignedToMe;
                    listDbStatusesMultiSel = DbStatus.GetByStatusIds(listofStatues.ConvertAll(x => Convert.ToInt32(x)));
                    listofStatusesMeAndAll = listDbStatusesMultiSel.Select(n => new { id = n.Complaint_Status_Id, name = n.Status }).Cast<object>().ToList();
                }
            }
            #endregion













            response.app = new ExpandoObject();
            response.app.id = 1;
            response.app.name = "Public app";
            response.app.listForms = new List<ExpandoObject>();


            List<DbProvince> listDbProvince = DbProvince.AllProvincesList().Where(n => n.Province_Name.ToLower().Contains("punjab")).ToList();
            List<DbDivision> listDbDivision = DbDivision.GetByProvinceId(1);
            List<DbDistrict> listDbDistrict = DbDistrict.GetByDivisionIdsStrAndGroupId(string.Join(",", listDbDivision.Select(s => s.Division_Id).ToList()), null).ToList();
            //List<DbTehsil> listDbTehsil = DbTehsil.GetByDistrictIdList(Utility.GetNullableIntList(listDbDistrict.Select(s => s.District_Id).ToList()), null);


            List<DbComplaintType> listCatogory = DbComplaintType.GetByCampaignIdAndGroupId(99, null);
            List<DbComplaintSubType> listSubcategory = DbComplaintSubType.GetByComplaintTypes(listCatogory.Select(n => n.Complaint_Category).ToList());




            var lastGroupFormItem = formGroups.Last();
            foreach (var group in formGroups)
            {
                dynamic forms = new ExpandoObject();
                forms.formTag = group.Key;
                forms.fields = new List<ExpandoObject>();

                var lastGroupFieldsItem = group.OrderBy(o => o.Priority).ToList().Last();

                foreach (var item in group.OrderBy(o => o.Priority).ToList())
                {
                    dynamic field = new ExpandoObject();

                    field.field_key = item.FieldKey;
                    field.field_text = item.FieldName;
                    field.field_type = Utility.GetEnumDisplayName((Config.DynamicControlType)(int)item.Control_Type);

                    dynamic settings = JObject.Parse(item.Setting);

                    field.field_hint = settings.field_hint;
                    field.field_editable = settings.field_isEditable;
                    field.field_focusable = settings.field_isFocusable;
                    field.field_required = settings.field_isRequired;
                    field.field_value = settings.field_value;




                    switch ((Config.DynamicControlType)(int)item.Control_Type)
                    {
                        case Config.DynamicControlType.Date:
                            field.field_input_type = settings.field_input_type;
                            switch (item.FieldKey)
                            {
                                case "listingStartDate":
                                    field.field_min_date = settings.field_min_date;
                                    field.field_max_date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).ToString("yyyy-MM-dd"); ;
                                    field.field_value = DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd");
                                    break;
                                case "listingEndDate":
                                    field.field_min_date = settings.field_min_date;
                                    field.field_max_date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).ToString("yyyy-MM-dd"); ;
                                    field.field_value = DateTime.Now.ToString("yyyy-MM-dd");
                                    break;
                            }
                            break;


                        case Config.DynamicControlType.TextBox:
                        case Config.DynamicControlType.Password:
                            field.text_min_length = settings.text_min_length;
                            field.field_input_type = settings.field_input_type;
                            field.field_special_case = settings.field_special_case;
                            field.text_length = settings.field_length;
                            field.text_lines = settings.field_lines;
                            break;





                        case Config.DynamicControlType.MultiSelectDropdown:
                            
                            switch (group.Key)
                            {
                                case "Listing":
                                    if (dbUser.Role_Id == Config.Roles.Stakeholder)
                                    {
                                        // Adding list type before multiselect status
                                        List<dynamic> listComplaintType = new List<dynamic> {
                                                new { id=(int)Config.StakeholderComplaintListingType.AssignedToMe, name="Assigned To Me"},
                                                new { id=(int)Config.StakeholderComplaintListingType.UptilMyHierarchy, name="All"}
                                            };
                                        dynamic listingField = new ExpandoObject();
                                        listingField.field_key = "listingType";
                                        listingField.field_text = "Listing type";
                                        listingField.field_type = "dropdown";
                                        listingField.field_hint = "Complaint listing type";
                                        listingField.field_editable = true;
                                        listingField.field_focusable = true;
                                        listingField.field_required = true;
                                        listingField.field_value = string.Join(",", new List<dynamic> { listComplaintType[0].id });
                                        listingField.field_options = JsonConvert.SerializeObject(GetOptionsList(listComplaintType, "id", "name", "parentId", null, "1"));
                                        listingField.field_group = 1;
                                        listingField.field_position_in_group = 1;
                                        listingField.field_hirarchy = 1;
                                        listingField.formTag = group.Key;
                                        forms.fields.Add(listingField);

                                        //var listStatuses = DbStatus.GetByStatusIds(listofStatues.ConvertAll(x => Convert.ToInt32(x)));
                                        field.field_group = 1;//settings.field_dropDown_Hierarchy;
                                        field.field_position_in_group = 2;//settings.field_position_in_group;
                                        field.field_hirarchy = 2;//settings.field_hirarchy;
                                        field.field_options = JsonConvert.SerializeObject(GetOptionsList(listofStatusesMeAndAll, "id", "name", "parentId", null, null));
                                        //field.field_value = string.Join(",", listStatuses.Select(s => s.Complaint_Status_Id).ToList());
                                        field.field_value = string.Join(",", listofStatusesMeAndAll.Where(n=>n.parentId== (int)Config.StakeholderComplaintListingType.AssignedToMe).Select(s => s.id).ToList());
                                    }
                                    else
                                    {
                                        field.field_group = 1;//settings.field_dropDown_Hierarchy;
                                        field.field_position_in_group = 1;//settings.field_position_in_group;
                                        field.field_hirarchy = 1;//settings.field_hirarchy;
                                        field.field_options = JsonConvert.SerializeObject(GetOptionsList(listofStatusesMeAndAll, "id", "name", null, null, null));
                                        field.field_value = string.Join(",", listofStatusesMeAndAll.Select(s => s.id).ToList());
                                    }
                                    break;
                            }
                            break;

                        case Config.DynamicControlType.DropDownList:

                            
                            switch (group.Key)
                            {
                                case "PublicUserSignupForm":
                                    if (item.FieldKey == "userProvince")
                                    {
                                        field.field_options = JsonConvert.SerializeObject(GetOptionsList(listDbProvince, "Province_Id", "Province_Name", null, null, "1"));
                                    }
                                    else if (item.FieldKey == "userDistrict")
                                    {
                                        listDbDistTemp = listDbDistrict;
                                        field.field_options = JsonConvert.SerializeObject(GetOptionsList(GetDistricts(listDbDistTemp, listDbDivision),
                                            "id", "name", "provId", null, "36"));
                                    }
                                    else if (item.FieldKey == "userGender")
                                    {
                                        field.field_options = settings.field_options;
                                    }
                                    field.field_group = settings.field_dropDown_Hierarchy;
                                    field.field_position_in_group = settings.field_position_in_group;
                                    field.field_hirarchy = settings.field_hirarchy;

                                    break;
                                case "PublicUserDCChiniotComplaintAddForm":
                                    if (item.FieldKey == "userProvince" || item.FieldKey == "complaintProvince")
                                    {
                                        field.field_options = JsonConvert.SerializeObject(GetOptionsList(listDbProvince, "Province_Id", "Province_Name", null, null, "1"));
                                    }
                                    else if (item.FieldKey == "userDistrict")
                                    {
                                        listDbDistTemp = listDbDistrict;
                                        field.field_options = JsonConvert.SerializeObject(GetOptionsList(GetDistricts(listDbDistTemp, listDbDivision),
                                            "id", "name", "provId", null, null));
                                    }
                                    else if (item.FieldKey == "complaintDistrict")
                                    {
                                        listDbDistTemp = listDbDistrict.Where(n => n.District_Id == 36).ToList();
                                        field.field_options = JsonConvert.SerializeObject(GetOptionsList(GetDistricts(listDbDistTemp, listDbDivision),
                                            "id", "name", "provId", null, "36"));
                                    }
                                    else if (item.FieldKey == "complaintTehsil")
                                    {
                                        List<DbTehsil> listTehsilSpecific = DbTehsil.GetByDistrictIdList(Utility.GetNullableIntList(listDbDistTemp.Select(s => s.District_Id).ToList()), null);
                                        field.field_options = JsonConvert.SerializeObject(GetOptionsList(listTehsilSpecific, "Tehsil_Id", "Tehsil_Name", "District_Id", null, null));
                                    }
                                    else if (item.FieldKey == "userGender")
                                    {
                                        field.field_options = settings.field_options;
                                    }
                                    else if (item.FieldKey == "complaintCategory")
                                    {
                                        field.field_options = JsonConvert.SerializeObject(GetOptionsList(listCatogory, "Complaint_Category", "Name", null, null, null));
                                    }
                                    else if (item.FieldKey == "complaintSubCategory")
                                    {
                                        field.field_options = JsonConvert.SerializeObject(GetOptionsList(listSubcategory, "Complaint_SubCategory", "Name", "Complaint_Type_Id", null, null));
                                    }
                                    field.field_group = settings.field_dropDown_Hierarchy;
                                    field.field_position_in_group = settings.field_position_in_group;
                                    field.field_hirarchy = settings.field_hirarchy;

                                    break;
                                
                            }
                            break;



                        case Config.DynamicControlType.FileAttachment:
                            break;
                    }
                    field.formTag = group.Key;
                    forms.fields.Add(field);
                }
                response.app.listForms.Add(forms);
            }
            response = ApiResponseHandlerMobile.SetSuccess(ApiResponseHandlerMobile.SetData(response), "Forms fetched successfully");
            return response;
        }


        public static List<dynamic> GetOptionsList(IEnumerable<object> listToConvert, string key, string value, string parentKey, string firstValue = null, object selectedValue = null)
        {
            List<dynamic> listDynamic = new List<dynamic>();
            dynamic d = null;
            foreach (object o in listToConvert)
            {
                d = new ExpandoObject();
                d.value = Utility.GetPropertyThroughReflection(o, key).ToString();
                d.text = Utility.GetPropertyThroughReflection(o, value).ToString();
                if (parentKey != null)
                {
                    object parentId = Utility.GetPropertyThroughReflection(o, parentKey);
                    if (parentId != null)
                    {
                        d.parentId = parentId.ToString();
                    }
                }

                d.selected = selectedValue != null ? Utility.GetPropertyThroughReflection(o, key).ToString() == selectedValue.ToString() : false;
                listDynamic.Add(d);
            }


            if (firstValue != null)
            {
                d = new ExpandoObject();
                d.Value = "-1";
                d.Text = firstValue;
                d.Insert(0, d);
            }
            return listDynamic;
        }


        public static List<object> GetDistricts(List<DbDistrict> listDistrict, List<DbDivision> listDbDivision)
        {
            List<object> listDistrctOfProvRef = new List<object>();
            foreach (DbDistrict dbDistrict in listDistrict)
            {
                //listDistrctOfProvRef
                int? provId = listDbDivision.Where(n => n.Division_Id == dbDistrict.Division_Id).FirstOrDefault().Province_Id;
                listDistrctOfProvRef.Add(new { id = dbDistrict.District_Id, name = dbDistrict.District_Name, provId = provId });

            }
            return listDistrctOfProvRef;
        }

        private static dynamic GetDistricts(List<DbProvince> provinces, object selectedValue = null)
        {
            dynamic options = new List<ExpandoObject>();
            foreach (var item in provinces)
            {
                var districts = DbDistrict.GetDistrictByProvinceAndGroup(item.Province_Id, null).Where(n => n.District_Name.ToLower().Contains("chiniot")).ToList();

                foreach (object o in districts)
                {
                    dynamic option = new ExpandoObject();
                    option.value = Utility.GetPropertyThroughReflection(o, "District_Id").ToString();
                    option.text = Utility.GetPropertyThroughReflection(o, "District_Name").ToString();
                    //option.selected = false;
                    option.parentId = item.Province_Id;

                    option.selected = selectedValue != null ? Utility.GetPropertyThroughReflection(o, "District_Id").ToString() == selectedValue.ToString() : false;
                    options.Add(option);
                }
            }
            return options;
        }
    }
}
