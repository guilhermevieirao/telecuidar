using MediatR;
using app.Application.Common.Models;
using app.Domain.Entities;
using app.Domain.Interfaces;
using app.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace app.Application.Specialties.Commands;

public class AssignSpecialtyToProfessionalCommandHandler : IRequestHandler<AssignSpecialtyToProfessionalCommand, Result<bool>>
{
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Specialty> _specialtyRepository;
    private readonly IRepository<UserSpecialty> _userSpecialtyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AssignSpecialtyToProfessionalCommandHandler(
        IRepository<User> userRepository,
        IRepository<Specialty> specialtyRepository,
        IRepository<UserSpecialty> userSpecialtyRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _specialtyRepository = specialtyRepository;
        _userSpecialtyRepository = userSpecialtyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(AssignSpecialtyToProfessionalCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            return Result<bool>.Failure("Usuário não encontrado.");
        }

        if (user.Role != UserRole.Profissional)
        {
            return Result<bool>.Failure("O usuário deve ser um profissional de saúde.");
        }

        var specialty = await _specialtyRepository.GetByIdAsync(request.SpecialtyId);
        if (specialty == null)
        {
            return Result<bool>.Failure("Especialidade não encontrada.");
        }

        // Verificar se já existe a associação
        var existingAssociation = await _userSpecialtyRepository.GetQueryable()
            .FirstOrDefaultAsync(
                us => us.UserId == request.UserId && us.SpecialtyId == request.SpecialtyId,
                cancellationToken);

        if (existingAssociation != null)
        {
            return Result<bool>.Failure("Este profissional já possui esta especialidade.");
        }

        var userSpecialty = new UserSpecialty
        {
            UserId = request.UserId,
            SpecialtyId = request.SpecialtyId,
            CreatedAt = DateTime.UtcNow
        };

        await _userSpecialtyRepository.AddAsync(userSpecialty);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
