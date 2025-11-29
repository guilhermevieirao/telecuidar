using MediatR;
using app.Application.Common.Models;
using app.Domain.Entities;
using app.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.Application.Specialties.Commands.DeleteSpecialty;

public class DeleteSpecialtyCommandHandler : IRequestHandler<DeleteSpecialtyCommand, Result<bool>>
{
    private readonly IRepository<Specialty> _specialtyRepository;
    private readonly IRepository<UserSpecialty> _userSpecialtyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSpecialtyCommandHandler(
        IRepository<Specialty> specialtyRepository,
        IRepository<UserSpecialty> userSpecialtyRepository,
        IUnitOfWork unitOfWork)
    {
        _specialtyRepository = specialtyRepository;
        _userSpecialtyRepository = userSpecialtyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(DeleteSpecialtyCommand request, CancellationToken cancellationToken)
    {
        var specialty = await _specialtyRepository.GetByIdAsync(request.Id);
        if (specialty == null)
        {
            return Result<bool>.Failure("Especialidade não encontrada.");
        }

        // Remover todas as associações desta especialidade com profissionais
        var userSpecialties = await _userSpecialtyRepository.GetQueryable()
            .Where(us => us.SpecialtyId == request.Id)
            .ToListAsync(cancellationToken);

        foreach (var userSpecialty in userSpecialties)
        {
            await _userSpecialtyRepository.DeleteAsync(userSpecialty.Id);
        }

        await _specialtyRepository.DeleteAsync(request.Id);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
