using PITB.CMS_Common;
using System;
using System.Diagnostics;



namespace PITB.CMS_DB.Models
{

    public partial class DbWindowServiceError
    {

        #region HelperFunctions

        public static void SaveErrorLog(DbWindowServiceError dbWindowServiceError)
        {
            try
            {
                DBContextHelperLinq db = new DBContextHelperLinq();
                db.DbWindowServiceError.Add(dbWindowServiceError);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public static void SaveErrorLog(int errorId, string stackTraceStr, Exception exception, Config.ServiceType serviceType)
        {
            try
            {
                var st = new StackTrace(exception, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                var line = frame.GetFileLineNumber();

                DbWindowServiceError dbWindowSerciceError = new DbWindowServiceError();
                dbWindowSerciceError.StackTrace = stackTraceStr;
                dbWindowSerciceError.ErrorId = errorId;
                dbWindowSerciceError.ErrorStr = exception.Message.ToString() + " at Line : " + line +
                                                ((exception.InnerException != null)
                                                    ? exception.InnerException.Message
                                                    : "");
                dbWindowSerciceError.ServiceType = (int)serviceType;
                dbWindowSerciceError.CreationDateTime = DateTime.Now;

                DBContextHelperLinq db = new DBContextHelperLinq();
                db.DbWindowServiceError.Add(dbWindowSerciceError);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public static void SaveErrorLog(int errorId, string errorStr, string stackTrace, Config.ServiceType serviceType)
        {
            try
            {


                DbWindowServiceError dbWindowSerciceError = new DbWindowServiceError();
                dbWindowSerciceError.StackTrace = stackTrace;
                dbWindowSerciceError.ErrorId = errorId;
                dbWindowSerciceError.ErrorStr = errorStr;
                dbWindowSerciceError.ServiceType = (int)serviceType;
                dbWindowSerciceError.CreationDateTime = DateTime.Now;

                DBContextHelperLinq db = new DBContextHelperLinq();
                db.DbWindowServiceError.Add(dbWindowSerciceError);
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        #endregion

    }
}
