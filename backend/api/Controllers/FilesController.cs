using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MediatR;
using System.Security.Claims;
using app.Application.Files.Commands.UploadFile;
using app.Application.Files.Queries.GetMyFiles;
using Microsoft.EntityFrameworkCore;
using app.Domain.Entities;
using app.Domain.Interfaces;

namespace app.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IRepository<FileUpload> _fileRepository;
    private readonly IUnitOfWork _unitOfWork;

    public FilesController(IMediator mediator, IRepository<FileUpload> fileRepository, IUnitOfWork unitOfWork)
    {
        _mediator = mediator;
        _fileRepository = fileRepository;
        _unitOfWork = unitOfWork;
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile uploadedFile, [FromForm] string fileCategory, [FromForm] string? description, [FromForm] bool isPublic = false, [FromForm] int? relatedUserId = null)
    {
        var command = new UploadFileCommand
        {
            File = uploadedFile,
            FileCategory = fileCategory,
            Description = description,
            IsPublic = isPublic,
            RelatedUserId = relatedUserId,
            UploadedByUserId = GetCurrentUserId()
        };

        var result = await _mediator.Send(command);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("my-files")]
    public async Task<IActionResult> GetMyFiles([FromQuery] string? fileCategory, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var query = new GetMyFilesQuery
        {
            UserId = GetCurrentUserId(),
            FileCategory = fileCategory,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);

        if (result.IsSuccess)
        {
            return Ok(result);
        }

        return BadRequest(result);
    }

    [HttpGet("{id}/download")]
    public async Task<IActionResult> DownloadFile(int id)
    {
        var currentUserId = GetCurrentUserId();
        var file = await _fileRepository.GetQueryable()
            .FirstOrDefaultAsync(f => f.Id == id);

        if (file == null)
        {
            return NotFound(new { message = "Arquivo não encontrado" });
        }

        // Verificar permissão
        var hasAccess = file.IsPublic || 
                        file.UploadedByUserId == currentUserId || 
                        file.RelatedUserId == currentUserId ||
                        User.IsInRole("Administrador");

        if (!hasAccess)
        {
            return Forbid();
        }

        if (!System.IO.File.Exists(file.FilePath))
        {
            return NotFound(new { message = "Arquivo físico não encontrado" });
        }

        var memory = new MemoryStream();
        using (var stream = new FileStream(file.FilePath, FileMode.Open))
        {
            await stream.CopyToAsync(memory);
        }
        memory.Position = 0;

        return File(memory, file.ContentType, file.OriginalFileName);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFile(int id)
    {
        var currentUserId = GetCurrentUserId();
        var file = await _fileRepository.GetByIdAsync(id);

        if (file == null)
        {
            return NotFound(new { message = "Arquivo não encontrado" });
        }

        // Apenas quem fez upload ou admin pode excluir
        if (file.UploadedByUserId != currentUserId && !User.IsInRole("Administrador"))
        {
            return Forbid();
        }

        // Excluir arquivo físico
        if (System.IO.File.Exists(file.FilePath))
        {
            System.IO.File.Delete(file.FilePath);
        }

        // Excluir registro do banco
        await _fileRepository.DeleteAsync(file.Id);
        await _unitOfWork.SaveChangesAsync();

        return Ok(new { isSuccess = true, message = "Arquivo excluído com sucesso" });
    }
}
