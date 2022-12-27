using System.Collections.Generic;
using PSW.ITT.Data.DTO;
using PSW.ITT.Data.Entities;
using System.Data;
using System;

namespace PSW.ITT.Data.IRepositories
{
    public interface ILPCORegulationRepository : IRepository<LPCORegulation>
    {
        LPCORegulation CheckIfRecordAlreadyExistInTheSystem(string hsCode, string productCode, short tradeTranTypeID, int agencyID, string factor); //, DateTime EffectiveFromDt, DateTime EffectiveThruDt
        List<LPCORegulation> GetRegulationByProductAgencyLinkID(long productAgencyID);
    }
}
