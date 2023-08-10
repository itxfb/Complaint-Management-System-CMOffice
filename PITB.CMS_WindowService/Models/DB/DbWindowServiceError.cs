
using System.Diagnostics;
using PITB.CMS_WindowService.Helper;

namespace PITB.CMS_WindowService.Models.DB
{
    //using System;
    //using System.Collections.Generic;
    //using System.ComponentModel.DataAnnotations;
    //using System.ComponentModel.DataAnnotations.Schema;
    //using System.Linq;

    //[Table("PITB.WindowServiceError")]
    //public class DbWindowServiceError
    //{
    //    [Key]
    //    [Column("id")]
    //    public int Id { get; set; }

    //    public string StackTrace { get; set; }

    //    public int ErrorId { get; set; }

    //    public string ErrorStr { get; set; }

    //    public int ServiceType { get; set; }

    //    public DateTime? CreationDateTime { get; set; }

    //    #region HelperFunctions

    //    public static void SaveErrorLog(DbWindowServiceError dbWindowServiceError) 
    //    {
    //        try
    //        {
    //            DbContextHelperLinq db = new DbContextHelperLinq();
    //            db.DbWindowServiceErrors.Add(dbWindowServiceError);
    //            db.SaveChanges();
    //        }
    //        catch (Exception ex)
    //        {
                
    //        }
    //    }

    //    public static void SaveErrorLog(int errorId, string stackTraceStr, Exception exception, Config.ServiceType serviceType)
    //    {
    //        try
    //        {
    //            var st = new StackTrace(exception, true);
    //            // Get the top stack frame
    //            var frame = st.GetFrame(0);
    //            // Get the line number from the stack frame
    //            var line = frame.GetFileLineNumber();

    //            DbWindowServiceError dbWindowSerciceError = new DbWindowServiceError();
    //            dbWindowSerciceError.StackTrace = stackTraceStr;
    //            dbWindowSerciceError.ErrorId = errorId;
    //            dbWindowSerciceError.ErrorStr = exception.Message.ToString() + " at Line : " + line;
    //            dbWindowSerciceError.ServiceType = (int) serviceType;
    //            dbWindowSerciceError.CreationDateTime = DateTime.Now;

    //            DbContextHelperLinq db = new DbContextHelperLinq();
    //            db.DbWindowServiceErrors.Add(dbWindowSerciceError);
    //            db.SaveChanges();
    //        }
    //        catch (Exception ex)
    //        {

    //        }
    //    }

    //    public static void SaveErrorLog(int errorId, string errorStr, string stackTrace, Config.ServiceType serviceType)
    //    {
    //        try
    //        {
           

    //            DbWindowServiceError dbWindowSerciceError = new DbWindowServiceError();
    //            dbWindowSerciceError.StackTrace = stackTrace;
    //            dbWindowSerciceError.ErrorId = errorId;
    //            dbWindowSerciceError.ErrorStr = errorStr;
    //            dbWindowSerciceError.ServiceType = (int)serviceType;
    //            dbWindowSerciceError.CreationDateTime = DateTime.Now;

    //            DbContextHelperLinq db = new DbContextHelperLinq();
    //            db.DbWindowServiceErrors.Add(dbWindowSerciceError);
    //            db.SaveChanges();
    //        }
    //        catch (Exception ex)
    //        {

    //        }
    //    }

    //    #endregion

    //}
}
