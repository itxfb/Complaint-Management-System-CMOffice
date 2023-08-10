using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PITB.CRM.Public_Web.Contacts
{
    public interface ISocialAction<in T> where T:class
    {
        int PostComment(T obj);
    }
}