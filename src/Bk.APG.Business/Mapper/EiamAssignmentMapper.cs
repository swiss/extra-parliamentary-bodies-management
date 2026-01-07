using Bk.APG.Business.Dtos;
using Bk.APG.Business.Models;

namespace Bk.APG.Business.Mapper;

public static class EiamAssignmentMapper
{
    public static EiamAssignmentDto ToDto(EiamAssignment eiamAssignment, bool useDescription = false)
    {
        return new EiamAssignmentDto
        {
            Id = eiamAssignment.Id,
            Text = useDescription ? eiamAssignment.GetDescription() : eiamAssignment.GetText(),
            DepartmentId = eiamAssignment.DepartmentId
        };
    }
}
