using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using PSW.ITT.Data;
using System.Text.Json;
using AutoMapper;
using PSW.ITT.Common.Pagination;
using PSW.Common.Crypto;

namespace PSW.ITT.Service.Command
{
    public class CommandRequest
    {
        public JsonElement data { get; set; }
        public string methodId { get; set; }
        public IUnitOfWork UnitOfWork { get; set; }
        public ICryptoAlgorithm CryptoAlgorithm { get; set; }
        public IMapper _mapper { get; set; }
        public ServerPaginationModel pagination { get; set; }
        public IEnumerable<Claim> UserClaims { get; set; }
        public ClaimsPrincipal CurrentUser { get; set; }
        public int LoggedInUserRoleID { get; set; }
        public string CurrentUserName
        {
            get
            {
                return CurrentUser?.Claims?.First(claim => claim.Type == "username").Value;
            }
        }

        public string NTN
        {
            get
            {
                return CurrentUser?.Claims?.First(claim => claim.Type == "ntn").Value;
            }
        }

        public string EmailAddress
        {
            get
            {
                return CurrentUser?.Claims?.First(claim => claim.Type == "email").Value;
            }
        }
    }
}