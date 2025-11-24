using MediatR;
using app.Application.Common.Models;
using app.Application.Admin.DTOs;

namespace app.Application.Admin.Queries.GetUserStatistics;

public class GetUserStatisticsQuery : IRequest<Result<UserStatisticsDto>>
{
}
