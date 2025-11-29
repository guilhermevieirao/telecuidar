using MediatR;
using app.Application.Common.Models;
using app.Domain.Entities;
using app.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace app.Application.Specialties.Commands;

public class CreateSpecialtyCommandHandler : IRequestHandler<CreateSpecialtyCommand, Result<int>>
{
    private readonly IRepository<Specialty> _specialtyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSpecialtyCommandHandler(
        IRepository<Specialty> specialtyRepository,
        IUnitOfWork unitOfWork)
    {
        _specialtyRepository = specialtyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<int>> Handle(CreateSpecialtyCommand request, CancellationToken cancellationToken)
    {
        // Verificar se já existe uma especialidade com o mesmo nome
        var existingSpecialty = await _specialtyRepository.GetQueryable()
            .FirstOrDefaultAsync(
                s => s.Name.ToLower() == request.Name.ToLower(), 
                cancellationToken);

        if (existingSpecialty != null)
        {
            return Result<int>.Failure("Já existe uma especialidade com este nome.");
        }

        var specialty = new Specialty
        {
            Name = request.Name,
            Description = request.Description,
            Icon = request.Icon,
            CreatedAt = DateTime.UtcNow
        };

        await _specialtyRepository.AddAsync(specialty);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<int>.Success(specialty.Id);
    }
}
