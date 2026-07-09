using FluentResults;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace QubeFin.Payroll.Application.Payrolls.Commands
{
    public record UpdatePayrollCommand(Guid Id,
        string Name,
        string Code,
        Guid CategoryId,
        bool IsTaxable,
        bool IsPfapplicable,
        bool IsEsiapplicable,
        bool IsCtccomponent,
        bool IsActive,
        int DisplayOrder,
        Guid? LastModifiedBy) : IRequest<Result<UpdatePayrollResponse>>;
    public record UpdatePayrollResponse(bool Updated);
    internal class UpdatePayroll
    {
    }
}
