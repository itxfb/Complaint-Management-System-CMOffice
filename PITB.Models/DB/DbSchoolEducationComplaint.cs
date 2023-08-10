namespace PITB.CMS_Models.DB
{
    public class DbSchoolEducationComplaint//: DbComplaint
    {
        public DbSchoolsMapping DbSchoolMapping { get; set; }
        public DbComplaint DbComplaint { get; set; }
        //public DbComplaint DbComplaint { get; set; }


        // public DbSchoolEducationComplaint(DbComplaint dbComplaint) //: base(dbComplaint)
        //{
        //    //base = dbComplaint;
        //}
    }
}