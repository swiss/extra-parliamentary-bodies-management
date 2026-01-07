namespace Bk.APG.Business.Dtos;

public class AppointmentDecisionCreateDto : AppointmentDecisionModificationDto
{
    public Guid CommitteeId { get; set; }
}
