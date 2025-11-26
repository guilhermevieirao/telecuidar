using MediatR;
using app.Application.Reports.DTOs;

namespace app.Application.Reports.Queries.GetFilesReport;

public class GetFilesReportQuery : IRequest<FilesReportDto>
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}
