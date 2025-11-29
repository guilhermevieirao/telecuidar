using MediatR;
using app.Application.Common.Models;
using app.Domain.Entities;
using app.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.Application.Specialties.Commands;

public class RemoveSpecialtyFromProfessionalCommandHandler : IRequestHandler<RemoveSpecialtyFromProfessionalCommand, Result<bool>>
{
    private readonly IRepository<UserSpecialty> _userSpecialtyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveSpecialtyFromProfessionalCommandHandler(
        IRepository<UserSpecialty> userSpecialtyRepository,
        IUnitOfWork unitOfWork)
    {
        _userSpecialtyRepository = userSpecialtyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(RemoveSpecialtyFromProfessionalCommand request, CancellationToken cancellationToken)
    {
        var userSpecialty = await _userSpecialtyRepository.GetQueryable()
            .FirstOrDefaultAsync(
                us => us.UserId == request.UserId && us.SpecialtyId == request.SpecialtyId,
                cancellationToken);

        if (userSpecialty == null)
        {
            return Result<bool>.Failure("Associação não encontrada.");
        }

        await _userSpecialtyRepository.DeleteAsync(userSpecialty.Id);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
