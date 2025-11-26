using MediatR;
using app.Application.Reports.DTOs;

namespace app.Application.Reports.Queries.GetUsersReport;

public class GetUsersReportQuery : IRequest<UsersReportDto>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
