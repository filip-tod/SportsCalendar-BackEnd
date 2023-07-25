using SportCalendar.Common;
using SportCalendar.Model;
using SportCalendar.ServiceCommon;
using SportCalendar.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SportCalendar.WebApi.Controllers
{
    public class RoleController : ApiController
    {
        public RoleController(IRoleService roleService) 
        {
            RoleService = roleService;
        }
        protected IRoleService RoleService { get; set; }

        [HttpGet]
        [Authorize(Roles = "Super_admin")]
        public async Task<HttpResponseMessage> GetAllAsync(string orderBy = "Id", string sortOrder = "ASC", string searchQuery = null)
        {
            try
            {
                Sorting sorting = new Sorting() { OrderBy = orderBy, SortOrder = sortOrder };
                BaseFiltering filtering = new BaseFiltering(searchQuery, null, null, null, null);

                List<Role> rolesList = await RoleService.GetAllAsync(sorting, filtering);
               
                if (rolesList != null)
                {
                    List<RESTRole> restRoles = await RestRoles(rolesList);

                    return Request.CreateResponse(HttpStatusCode.OK, rolesList);
                };
                return Request.CreateResponse(HttpStatusCode.NotFound, "Roles list is empty!");
            }
            catch
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Oops something went wrong!!");
            }
        }

        private static async Task<List<RESTRole>> RestRoles(List<Role> rolesList)
        {
            List<RESTRole> restRoles = new List<RESTRole>();
            
            for (int count = 0; count < rolesList.Count; count++)
            {
                restRoles.Add(
                    new RESTRole
                    {
                        Id = rolesList[count].Id,
                        Access = rolesList[count].Access,
                        IsActive = rolesList[count].IsActive,
                    });
            }
            return restRoles;
        }

    }
}
