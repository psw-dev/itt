using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using psw.itt.data;
using System.Text.Json;
using AutoMapper;
using psw.itt.common.Pagination;
using PSW.Common.Crypto;
using Microsoft.AspNetCore.Http;

namespace psw.itt.service.Command
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
        public string roleCode { get; set; }
        public IFormFile file { get; set; }
        public long fileId { get; set; }
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