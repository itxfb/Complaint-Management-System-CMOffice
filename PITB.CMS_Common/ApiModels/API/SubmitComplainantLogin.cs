namespace PITB.CMS_Common.ApiModels.API
{
    public class SubmitComplainantLogin
    {
        public string PersonName { get; set; }
        private string _cnic;

        public string Cnic
        {
            get { return (string.IsNullOrEmpty(_cnic)) ? string.Empty : _cnic.Trim(); }
            set { _cnic = value; }
        }
        
        
        public string MobileNo { get; set; }
        public string ImeiNo { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }


        public string Email { get; set; }
        public string FullName { get { return string.Format("{0} {1}", FirstName, LastName); } }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsExternalLogin
        {
            get
            {
                return !string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(UserProvider);
            }
        }
        private string _userId;

        public string UserId
        {
            get { return (string.IsNullOrEmpty(_userId)) ? string.Empty : _userId.Trim(); }
            set { _userId = value; }
        }

        private string _userProvider;

        public string UserProvider
        {
            get { return (string.IsNullOrEmpty(_userProvider)) ? string.Empty : _userProvider.Trim(); }
            set { _userProvider = value; }
        }
        
        

        public bool ModelIsValid()
        {
            bool isValid = true;
            if (IsExternalLogin)
            {
                if (
                    //string.IsNullOrEmpty(ImeiNo) ||
                    string.IsNullOrEmpty(UserId) ||
                    string.IsNullOrEmpty(UserProvider) ||
                    string.IsNullOrEmpty(FirstName)
                    )
                    isValid = false;
            }
            else
            {
                if (
                    //string.IsNullOrEmpty(this.ImeiNo) ||
                    //string.IsNullOrEmpty(Cnic) ||
                    string.IsNullOrEmpty(MobileNo) ||
                    string.IsNullOrEmpty(PersonName)
                    )
                    isValid = false;

            }
            return isValid;
        }

    }
}