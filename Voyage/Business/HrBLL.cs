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

        public async Task<List<ManagePersonnelDTO>> GetPersonnel()
        {
            return await _hrDAL.GetPersonnel();
        }

        public async Task<List<ManageRolesDTO>> GetRoles()
        {
            return await _hrDAL.GetRoles();
        }

        public async Task<List<ManageDepartmentsDTO>> GetDepartments()
        {
            return await _hrDAL.GetDepartments();
        }

        public async Task<List<TeamDTO>> GetTeams()
        {
            return await _hrDAL.GetTeams();
        }

        public async Task<List<ManagePermissionsDTO>> GetPermissions()
        {
            return await _hrDAL.GetPermissions();
        }

        public async Task<List<TeamMemberDTO>> GetTeamMembers()
        {
            return await _hrDAL.GetTeamMembers();
        }

        public async Task<ManageTeamsDTO> GetAssignedTeams()
        {
            ManageTeamsDTO dto = new ManageTeamsDTO();
            List<TeamMemberDTO> list = new List<TeamMemberDTO>();

            var personnel = await GetPersonnel();
            var teamMembers = await GetTeamMembers();

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
            dto.Teams = await GetTeams();
            return dto;

        }

        public async Task SaveRoles(List<string> roles)
        {
            IgnoreAddDefaultRoles(ref roles);
            List<IdentityRole> rolesToSave = roles.Select(r => new IdentityRole(r)).ToList();
            await _hrDAL.SaveRoles(rolesToSave);
        }

        public async Task SaveDepartments(List<string> departments)
        {
            await _hrDAL.SaveDepartments(departments);
        }

        public async Task SavePermissions(List<string> permissions)
        {
            await _hrDAL.SavePermissions(permissions);
        }

        public async Task<List<TeamDTO>> SaveTeams(List<string> teams)
        {
            var teamsToSave = AddDefaultTeams(teams);

            await _hrDAL.SaveTeams(teamsToSave);
            return await GetTeams();
        }

        public async Task SaveTeamMembers(List<TeamDTO> teamMembers)
        {
            await _hrDAL.SaveTeamMembers(teamMembers);
        }

        private void IgnoreAddDefaultRoles(ref List<string> roles)
        {
            List<string> rolesToCompare = roles.Select(r => r.ToUpper()).ToList();
            List<string> defaultRoles = Enum.GetNames<Constants.DefaultRoles>().ToList();

            foreach (var role in defaultRoles)
            {
                if (rolesToCompare.Equals(role.ToUpper()))
                    roles.Remove(role);
            }
        }

        private List<string> AddDefaultTeams(List<string> teams)
        {
            List<string> teamsToSave = new List<string>();

            //ensure Unassigned is the first entry.
            if (!teams.Contains("Unassigned"))
            {
                teamsToSave.Add("Unassigned");
            }
            else
            {
                teams.Remove("Unassigned");
                teamsToSave.Add("Unassigned");
            }

            teamsToSave.AddRange(teams);

            return teamsToSave;
        }
    }
}
