using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CMS_Common.ApiModels.API.Datapack
{
    public class ClosureAndTimelinessParameterModel
    {
        public const string SchoolLevelAll = "H.Sec.,High,Middle,Primary,sMosque";
        public const string SchoolGenderAll = "Male,Female";
        public const string AssigneeAll = "AEO,CEO,DDEO,DEO(EE),DEO(SE),HT";
        public const string DefaultOrderByParameter = "Closurerate";
        public const string DefaultOrderBy = "ASC";

        private string _startDate = DateTime.MinValue.ToString("MM/dd/yyyy");
        private string _endDate = DateTime.Now.ToString("MM/dd/yyyy");
        private string _district = "All";
        private string _tehsil = "-1";
        private string _markaz = "-1";
        private string _schoolLevel = "All";
        private string _schoolGender = "All";
        private string _assignee = "All";
        private string _orderbyparameter = "-1";
        private string _orderby = "-1";
        public string District { get { return _district; } set { if (value != null) { _district = value; } } }
        public string Tehsil { get { return _tehsil; } set { if (value != null) { _tehsil = value; } } }
        public string Markaz { get { return _markaz; } set { if (value != null) { _markaz = value; } } }
        public string SchoolLevel { get { return _schoolLevel; } set { if (value != null) { _schoolLevel = value; } } }
        public string SchoolGender { get { return _schoolGender; } set { if (value != null) { _schoolGender = value; } } }
        public string Assignee { get { return _assignee; } set { if (value != null) { _assignee = value; } } }
        public string StartDate { get { return _startDate; } set { if (value != null) { _startDate = value; } } }
        public string EndDate { get { return _endDate; } set { if (value != null) { _endDate = value; } } }
        public string OrderByParam { get { return _orderbyparameter; } set { if (value != null) { _orderbyparameter = value; } } }
        public string OrderBy { get { return _orderby; } set { if (value != null) { _orderby = value; } } }
    }
}