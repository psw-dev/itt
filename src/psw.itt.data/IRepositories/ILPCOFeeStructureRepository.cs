/*This code is a generated one , Change the source code of the generator if you want some change in this code
You can find the source code of the code generator from here -> https://git.psw.gov.pk/unais.vayani/DalGenerator*/
using System.Collections.Generic;
using PSW.ITT.Data.DTO;
using PSW.ITT.Data.Entities;
using System.Collections.Generic;

namespace PSW.ITT.Data.IRepositories
{
    public interface ILPCOFeeStructureRepository : IRepository<LPCOFeeStructure>
    {
        List<LPCOFeeStructure> GetFeeConfig(int id);

    }
}
