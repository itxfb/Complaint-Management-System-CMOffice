using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PITB.CMS_Models.View.ClientMesages;

namespace PITB.CMS_Handlers.View.Account
{
    public class VmRole : ClientMessage
    {
        //public List<DbUsers> ListDbUsers { get; set; }

        public List<VmRoleEntry> ListRoleEntries { get; set; }

        public VmRoleEntry SelectedValue  { get; set; }

        public VmRole()
        {
            
        }

        public VmRole(List<DbUsers> listDbUsers, int selectedId )
        {
            ListRoleEntries = VmRoleEntry.GetRoleEntries(listDbUsers, selectedId);
        }
        
        
        public class VmRoleEntry
        {
            public int Id { get; set; }

            public string Value { get; set; }

            public bool IsSelected { get; set; }


            public VmRoleEntry()
            {
                
            }

            public static List<VmRoleEntry> GetRoleEntries(List<DbUsers> listDbUser, int selectedId)
            {
                List<VmRoleEntry> listRoleEntries = new List<VmRoleEntry>();
                VmRoleEntry vmRoleEntry = null;
                foreach (DbUsers dbUser in listDbUser)
                {
                    vmRoleEntry = new VmRoleEntry();
                    vmRoleEntry.Id = dbUser.User_Id;
                    vmRoleEntry.Value = dbUser.Designation + "["+DbUsers.GetHierarchyVal(dbUser)+"]";
                    if (dbUser.User_Id == selectedId)
                    {
                        vmRoleEntry.IsSelected = true;
                    }
                    else
                    {
                        vmRoleEntry.IsSelected = false;
                    }
                    listRoleEntries.Add(vmRoleEntry);
                }
                return listRoleEntries;
            }
        }
    
    }
}