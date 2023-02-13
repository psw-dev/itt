using System;
using System.Text.Json.Serialization;

namespace PSW.ITT.Data.DTO
{
    public class ProductCodeWithAgencyLink
    {   //ProductCode Table
        public long? SerialID { get; set; }
        public long ID { get; set; }
        public string HSCode { get; set; }
        public string HSCodeExt { get; set; }
        public string ProductCode { get; set; }
        public long ProductCodeChapterID { get; set; }
        public string ChapterCode { get; set; }
        public string Description { get; set; }
        public short TradeTranTypeID { get; set; }
        public long? ProductCodeSheetUploadHistoryID { get; set; }
        public DateTime EffectiveFromDt { get; set; }
        public DateTime EffectiveThruDt { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public bool? IsActive { get; set; }
        public bool? Regulated { get; set; }

        //ProductCodeAgencyLink Table
        public long ProductCodeAgencyLinkID  { get; set; }
        public short AgencyID  { get; set; }
        public DateTime ProductCodeAgencyLinkEffectiveFromDt  { get; set; }
        public DateTime ProductCodeAgencyLinkEffectiveThruDt  { get; set; }
        public DateTime RegulationEffectiveFromDt  { get; set; }
        public DateTime RegulationEffectiveThruDt  { get; set; }
        public bool IsActive  { get; set; }
        public bool SoftDelete  { get; set; }

    }
}
