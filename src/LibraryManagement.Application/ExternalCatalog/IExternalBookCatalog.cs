namespace LibraryManagement.Application.ExternalCatalog;

public interface IExternalBookCatalog
{
    Task<ExternalBookCatalogDto?> GetByIsbnAsync(string isbn, CancellationToken cancellationToken = default);
}
