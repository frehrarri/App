using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Voyage.Data;
using Voyage.Data.TableModels;
using Voyage.Models.DTO;
using Voyage.Utilities;
using static Voyage.Utilities.CustomAttributes;

namespace Voyage.Business
{
    public class HrBLL
    {
        private readonly HrDAL _hrDAL;

        public HrBLL(HrDAL hrDAL)
        {
            _hrDAL = hrDAL;
        }

        public async Task<List<ManagePersonnelDTO>> GetPersonnel(int companyId)
        {
            return await _hrDAL.GetPersonnel(companyId);
        }

        public async Task<List<ManageRolesDTO>> GetRoles(int companyId)
        {
            return await _hrDAL.GetRoles(companyId);
        }

        public async Task<List<ManageDepartmentsDTO>> GetDepartments(int companyId)
        {
            return await _hrDAL.GetDepartments(companyId);
        }

        public async Task<List<TeamDTO>> GetTeams(int companyId)
        {
            return await _hrDAL.GetTeams(companyId);
        }

        public async Task<List<AssignTeamDTO>> GetAssignTeam(string teamKey, int companyId)
        {
            return await _hrDAL.GetAssignTeam(teamKey, companyId);
        }

        public async Task<List<ManagePermissionsDTO>> GetPermissions()
        {
            return await _hrDAL.GetPermissions();
        }


        //public async Task<ManageTeamsDTO> GetAssignedTeams(int companyId)
        //{
        //    ManageTeamsDTO dto = new ManageTeamsDTO();
        //    List<TeamMemberDTO> list = new List<TeamMemberDTO>();

        //    var personnel = await GetPersonnel(companyId);
        //    dto.Teams = await GetTeams(companyId);
        //    return dto;
        //}

        public async Task<bool> SavePersonnel(List<ManagePersonnelDTO> personnel, int companyId)
        {
            return await _hrDAL.SavePersonnel(personnel, companyId);
        }

        public async Task<bool> SaveRoles(List<ManageRolesDTO> roles, int companyId)
        {
            return await _hrDAL.SaveRoles(roles, companyId);
        }

        public async Task<List<String>> SaveDepartments(List<DepartmentDTO> departments, int companyId)
        {
            return await _hrDAL.SaveDepartments(departments, companyId);
        }

        public async Task SavePermissions(List<string> permissions)
        {
            await _hrDAL.SavePermissions(permissions);
        }

        public async Task<List<string>> SaveTeams(List<TeamDTO> teams, int companyId)
        {
            /*await GetTeams(companyId);*/
            return await _hrDAL.SaveTeams(teams, companyId);
        }

        public async Task AssignTeamMembers(List<AssignTeamDTO> dto, int companyId)
        {
            await _hrDAL.AssignTeamMembers(dto, companyId);
        }

        public async Task<List<AssignDepartmentDTO>> GetAssignedDepartmentTeams(string departmentKey, int companyId)
        {
            return await _hrDAL.GetAssignedDepartmentTeams(departmentKey, companyId);
        }

        public async Task SaveAssignDepartmentTeams(List<AssignDepartmentDTO> dto, int companyId)
        {
            await _hrDAL.SaveAssignDepartmentTeams(dto, companyId);
        }

        public async Task<List<AssignDepartmentDTO>> GetAssignedDepartmentUsers(string departmentKey, int companyId)
        {
            return await _hrDAL.GetAssignedDepartmentUsers(departmentKey, companyId);
        }

        public async Task SaveAssignDepartmentUsers(List<AssignDepartmentDTO> dto, int companyId)
        {
            await _hrDAL.SaveAssignDepartmentUsers(dto, companyId);
        }

    }
}
