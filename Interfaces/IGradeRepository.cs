using Siemens.Internship2026.GradeBook.Models;

namespace Siemens.Internship2026.GradeBook.Interfaces;

public interface IGradeRepository
{
    Task<IReadOnlyCollection<Grade>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Grade?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}