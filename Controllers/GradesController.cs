using Microsoft.AspNetCore.Mvc;
using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class GradesController : ControllerBase
{
    private readonly IGradeService _gradeService;

    public GradesController(IGradeService gradeService)
    {
        _gradeService = gradeService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<Grade>>> GetAll(
        CancellationToken cancellationToken)
    {
        var grades = await _gradeService.GetAllAsync(cancellationToken);

        return Ok(grades);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Grade>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return BadRequest("Id must be a positive integer.");
        }

        var grade = await _gradeService.GetByIdAsync(id, cancellationToken);

        if (grade is null)
        {
            return NotFound($"Grade with id {id} was not found.");
        }

        return Ok(grade);
    }

    [HttpGet("passing-active")]
    public async Task<ActionResult<IReadOnlyCollection<Grade>>> GetFirstPassingActiveGrades(
        [FromQuery] int count,
        CancellationToken cancellationToken)
    {
        if (count <= 0)
        {
            return BadRequest("Count must be greater than zero.");
        }

        var grades = await _gradeService.GetFirstPassingActiveGradesAsync(
            count,
            cancellationToken);

        return Ok(grades);
    }
}