using Siemens.Internship2026.GradeBook.Interfaces;
using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Services;

public sealed class GradeService : IGradeService
{
    private const decimal PassingGrade = 5m;

    private readonly IGradeRepository _gradeRepository;

    public GradeService(IGradeRepository gradeRepository)
    {
        _gradeRepository = gradeRepository;
    }

    public Task<IReadOnlyCollection<Grade>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return _gradeRepository.GetAllAsync(cancellationToken);
    }

    public Task<Grade?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _gradeRepository.GetByIdAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Grade>> GetFirstPassingActiveGradesAsync(
        int count,
        CancellationToken cancellationToken = default)
    {
        if (count <= 0)
        {
            return Array.Empty<Grade>();
        }

        var grades = await _gradeRepository.GetAllAsync(cancellationToken);

        return grades
            .Where(grade => grade.IsActive && grade.Value >= PassingGrade)
            .Take(count)
            .ToList();
    }
}