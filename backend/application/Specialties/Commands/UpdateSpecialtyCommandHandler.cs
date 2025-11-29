using MediatR;
using app.Application.Common.Models;
using app.Domain.Entities;
using app.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.Application.Specialties.Commands;

public class UpdateSpecialtyCommandHandler : IRequestHandler<UpdateSpecialtyCommand, Result<bool>>
{
    private readonly IRepository<Specialty> _specialtyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSpecialtyCommandHandler(
        IRepository<Specialty> specialtyRepository,
        IUnitOfWork unitOfWork)
    {
        _specialtyRepository = specialtyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateSpecialtyCommand request, CancellationToken cancellationToken)
    {
        var specialty = await _specialtyRepository.GetByIdAsync(request.Id);
        if (specialty == null)
        {
            return Result<bool>.Failure("Especialidade não encontrada.");
        }

        // Verificar se já existe outra especialidade com o mesmo nome
        var existingSpecialty = await _specialtyRepository.GetQueryable()
            .FirstOrDefaultAsync(
                s => s.Name.ToLower() == request.Name.ToLower() && s.Id != request.Id,
                cancellationToken);

        if (existingSpecialty != null)
        {
            return Result<bool>.Failure("Já existe outra especialidade com este nome.");
        }

        specialty.Name = request.Name;
        specialty.Description = request.Description;
        specialty.Icon = request.Icon;
        specialty.UpdatedAt = DateTime.UtcNow;

        await _specialtyRepository.UpdateAsync(specialty);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
