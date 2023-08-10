namespace PITB.CRM_API.Models.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PITB.Complaints")]
    public partial class Complaint
    {
        [Key]
        [Column(Order = 0)]
        public int Id { get; set; }

        public int? Person_Id { get; set; }

        [StringLength(400)]
        public string Person_Name { get; set; }

        [StringLength(50)]
        public string Person_Contact { get; set; }

        [StringLength(50)]
        public string Person_Cnic { get; set; }

        public int? Person_Province_Id { get; set; }

        [StringLength(200)]
        public string Person_Province_Name { get; set; }

        public int? Person_Division_Id { get; set; }

        [StringLength(200)]
        public string Person_Division_Name { get; set; }

        public int? Person_District_Id { get; set; }

        [StringLength(200)]
        public string Person_District_Name { get; set; }

        public int? Person_Tehsil_Id { get; set; }

        [StringLength(200)]
        public string Person_Tehsil_Name { get; set; }

        public int? Person_Uc_Id { get; set; }

        [StringLength(200)]
        public string Person_Uc_Name { get; set; }

        public int? Complaint_Type { get; set; }

        public int? Department_Id { get; set; }

        [StringLength(200)]
        public string Department_Name { get; set; }

        public int? Complaint_Category { get; set; }

        [StringLength(200)]
        public string Complaint_Category_Name { get; set; }

        public int? Complaint_SubCategory { get; set; }

        [StringLength(200)]
        public string Complaint_SubCategory_Name { get; set; }

        public int? Compaign_Id { get; set; }

        [StringLength(200)]
        public string Campaign_Name { get; set; }

        public int? Province_Id { get; set; }

        [StringLength(200)]
        public string Province_Name { get; set; }

        public int? Division_Id { get; set; }

        [StringLength(200)]
        public string Division_Name { get; set; }

        public int? District_Id { get; set; }

        [StringLength(200)]
        public string District_Name { get; set; }

        public int? Tehsil_Id { get; set; }

        [StringLength(200)]
        public string Tehsil_Name { get; set; }

        public int? UnionCouncil_Id { get; set; }

        [StringLength(200)]
        public string UnionCouncil_Name { get; set; }

        public int? Ward_Id { get; set; }

        [StringLength(200)]
        public string Ward_Name { get; set; }

        public int? Agent_Id { get; set; }

        [StringLength(500)]
        public string Complaint_Address { get; set; }

        [StringLength(500)]
        public string Business_Address { get; set; }

        public string Complaint_Remarks { get; set; }

        public int? Complaint_Status_Id { get; set; }

        [StringLength(100)]
        public string Complaint_Status { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? Complaint_Computed_Status_Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [StringLength(100)]
        public string Complaint_Computed_Status { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? Complaint_Computed_Hierarchy_Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [StringLength(20)]
        public string Complaint_Computed_Hierarchy { get; set; }

        public DateTime? Created_Date { get; set; }

        public int? Created_By { get; set; }

        public DateTime? Complaint_Assigned_Date { get; set; }

        public DateTime? Completed_Date { get; set; }

        public DateTime? Updated_Date { get; set; }

        public int? Updated_By { get; set; }

        [Key]
        [Column(Order = 1)]
        public bool Is_Deleted { get; set; }

        public DateTime? Date_Deleted { get; set; }

        public int? Deleted_By { get; set; }

        public string Reason_ToDelete { get; set; }

        public string Agent_Comments { get; set; }

        public int? Status_ChangedBy { get; set; }

        [StringLength(200)]
        public string Status_ChangedBy_Name { get; set; }

        public DateTime? StatusChangedDate_Time { get; set; }

        public int? StatusChangedBy_RoleId { get; set; }

        public int? StatusChangedBy_HierarchyId { get; set; }

        public int? StatusChangedBy_User_HierarchyId { get; set; }

        [StringLength(1000)]
        public string StatusChangedComments { get; set; }

        public DateTime? Dt1 { get; set; }

        public int? SrcId1 { get; set; }

        public DateTime? Dt2 { get; set; }

        public int? SrcId2 { get; set; }

        public DateTime? Dt3 { get; set; }

        public int? SrcId3 { get; set; }

        public DateTime? Dt4 { get; set; }

        public int? SrcId4 { get; set; }

        public DateTime? Dt5 { get; set; }

        public int? SrcId5 { get; set; }

        public DateTime? Dt6 { get; set; }

        public int? SrcId6 { get; set; }

        public DateTime? Dt7 { get; set; }

        public int? SrcId7 { get; set; }

        public DateTime? Dt8 { get; set; }

        public int? SrcId8 { get; set; }

        public DateTime? Dt9 { get; set; }

        public int? SrcId9 { get; set; }

        public DateTime? Dt10 { get; set; }

        public int? SrcId10 { get; set; }

        public int? MaxLevel { get; set; }

        public bool? IsTransferred { get; set; }

        public int? ComplaintSrc { get; set; }

        public int? Complainant_Remark_Id { get; set; }

        [StringLength(2000)]
        public string Complainant_Remark_Str { get; set; }

        public int? UserSrcId1 { get; set; }

        public int? UserSrcId2 { get; set; }

        public int? UserSrcId3 { get; set; }

        public int? UserSrcId4 { get; set; }

        public int? UserSrcId5 { get; set; }

        public int? UserSrcId6 { get; set; }

        public int? UserSrcId7 { get; set; }

        public int? UserSrcId8 { get; set; }

        public int? UserSrcId9 { get; set; }

        public int? UserSrcId10 { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? Complaint_Computed_User_Hierarchy_Id { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [StringLength(100)]
        public string Computed_Remaining_Time_To_Escalate { get; set; }

        public int? TableRefId { get; set; }

        public int? TableRowRefId { get; set; }

        public int? UserCategoryId1 { get; set; }

        public int? UserCategoryId2 { get; set; }

        [StringLength(2000)]
        public string RefField1 { get; set; }

        [StringLength(2000)]
        public string RefField2 { get; set; }

        [StringLength(2000)]
        public string RefField3 { get; set; }

        [StringLength(2000)]
        public string RefField4 { get; set; }

        [StringLength(2000)]
        public string RefField5 { get; set; }

        [StringLength(2000)]
        public string RefField6 { get; set; }

        [StringLength(2000)]
        public string RefField7 { get; set; }

        [StringLength(2000)]
        public string RefField8 { get; set; }

        [StringLength(2000)]
        public string RefField9 { get; set; }

        [StringLength(2000)]
        public string RefField10 { get; set; }
    }
}
