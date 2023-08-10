namespace PITB.CMS_Common.Models
{
    public partial class DbSchoolEducationComplaint//: DbComplaint
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