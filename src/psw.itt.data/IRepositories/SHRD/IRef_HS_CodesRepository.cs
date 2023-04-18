using System.Collections.Generic;
using PSW.ITT.Data.DTO;
using System;
using PSW.ITT.Data.Entities;
namespace PSW.ITT.Data.IRepositories
{
    public interface IRef_HS_CodesRepository : IRepository<Ref_HS_Codes>
    {
        Ref_HS_Codes GetHsCodeList(string hsCode);
    }
}
