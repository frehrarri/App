using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Voyage.Data;
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

        public async Task<List<ManagePermissionsDTO>> GetPermissions()
        {
            return await _hrDAL.GetPermissions();
        }

        public async Task<List<TeamMemberDTO>> GetTeamMembers(int companyId)
        {
            return await _hrDAL.GetTeamMembers(companyId);
        }

        public async Task<ManageTeamsDTO> GetAssignedTeams(int companyId)
        {
            ManageTeamsDTO dto = new ManageTeamsDTO();
            List<TeamMemberDTO> list = new List<TeamMemberDTO>();

            var personnel = await GetPersonnel(companyId);
            var teamMembers = await GetTeamMembers(companyId);

            //check each person in person for a reference to teams in the teamMember
            //if there is not a result then add the person to the default team member list
            foreach (var person in personnel)
            {
                if (!teamMembers.Any(e => e.EmployeeId == person.EmployeeId))
                {
                    TeamMemberDTO teamMemberDTO = new TeamMemberDTO();
                    teamMemberDTO.FirstName = person.FirstName;
                    teamMemberDTO.LastName = person.LastName;
                    teamMemberDTO.Username = person.Username;
                    teamMemberDTO.Email = person.Email;
                    teamMemberDTO.PhoneNumber = person.PhoneNumber;
                    list.Add(teamMemberDTO);
                }
            }

            dto.TeamMembers = list; 
            dto.Teams = await GetTeams(companyId);
            return dto;

        }

        public async Task SaveRoles(List<RoleDTO> roles)
        {
            //IgnoreAddDefaultRoles(ref roles);
            await _hrDAL.SaveRoles(roles);
        }

        public async Task SaveDepartments(List<DepartmentDTO> departments, int companyId)
        {
            await _hrDAL.SaveDepartments(departments, companyId);
        }

        public async Task SavePermissions(List<string> permissions)
        {
            await _hrDAL.SavePermissions(permissions);
        }

        public async Task<List<TeamDTO>> SaveTeams(List<TeamDTO> teams, int companyId)
        {
            await _hrDAL.SaveTeams(teams, companyId);
            return await GetTeams(companyId);
        }

        public async Task SaveTeamMembers(List<TeamDTO> teamMembers)
        {
            //await _hrDAL.SaveTeamMembers(teamMembers);

        }

        private void IgnoreAddDefaultRoles(ref List<RoleDTO> roles)
        {
            List<string> rolesToCompare = roles.Select(r => r.Name.ToUpper()).ToList();
            List<string> defaultRoles = Enum.GetNames<Constants.DefaultRoles>().ToList();

            foreach (var role in defaultRoles)
            {
                if (rolesToCompare.Contains(role.ToUpper()))
                    roles.RemoveAll(r => r.Name == role);
            }
        }

        //private List<string> AddDefaultTeams(List<string> teams)
        //{
        //    List<string> teamsToSave = new List<string>();

        //    //ensure Unassigned is the first entry.
        //    if (!teams.Contains("Unassigned"))
        //    {
        //        teamsToSave.Add("Unassigned");
        //    }
        //    else
        //    {
        //        teams.Remove("Unassigned");
        //        teamsToSave.Add("Unassigned");
        //    }

        //    teamsToSave.AddRange(teams);

        //    return teamsToSave;
        //}
    }
}
