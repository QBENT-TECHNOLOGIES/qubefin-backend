using QubeFin.Global.Persistence.Repositories;

namespace QubeFin.Global.Application.SurveyUnit
{
    // Rules for the Credit Officers of a Group Survey. Used by the Create and Update commands.
    public static class GroupSurveyRules
    {
        // Returns the first problem found, or null when every Credit Officer can be assigned.
        // employeeIdsToCheck: only the officers that are new on this survey (officers already on it are not checked again).
        public static async Task<string?> CheckCreditOfficersAsync(
            ISurveyRepository surveyRepository,
            ISurveyCommitteeRepository committeeRepository,
            Guid branchId,
            Guid creatorEmployeeId,
            IEnumerable<Guid> employeeIdsToCheck)
        {
            var creditOfficers = await surveyRepository.GetCreditOfficersOfBranchAsync(branchId);

            foreach (var employeeId in employeeIdsToCheck)
            {
                // The person in charge cannot assign himself.
                if (employeeId == creatorEmployeeId)
                    return "You cannot assign yourself as a Credit Officer.";

                var officer = creditOfficers.FirstOrDefault(c => c.EmployeeId == employeeId);
                if (officer is null)
                    return "Only Credit Officers of this branch can be assigned.";

                if (await committeeRepository.AnyActiveMemberAsync(new[] { employeeId }))
                    return $"{officer.EmployeeName} is a Survey Committee member and cannot be assigned.";

                if (await surveyRepository.IsAssignedToOpenSurveyAsync(employeeId))
                    return $"{officer.EmployeeName} is already assigned to an open survey.";
            }
            return null;
        }
    }
}
