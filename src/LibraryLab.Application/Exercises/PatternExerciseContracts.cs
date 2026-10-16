using LibraryLab.Application.Contracts;
using LibraryLab.Application.Services;
using LibraryLab.Domain.Entities;
using LibraryLab.Domain.Enums;

namespace LibraryLab.Application.Exercises;

// ============================================================
// 1. FACTORY METHOD
// ============================================================

public interface ILoanFactoryExercise
{
    Loan Create(CreateLoanCommand command, ResourceKind resourceKind);
}

public sealed class LoanFactoryExercise : ILoanFactoryExercise
{
    public Loan Create(
        CreateLoanCommand command,
        ResourceKind resourceKind)
    {
        ArgumentNullException.ThrowIfNull(command);

        if (!Enum.IsDefined(resourceKind))
        {
            throw new ArgumentOutOfRangeException(
                nameof(resourceKind),
                "El tipo de recurso no es válido.");
        }

        return new Loan(
            Guid.NewGuid(),
            command.UserId,
            command.ResourceId,
            resourceKind,
            DateTimeOffset.UtcNow,
            command.DueAt);
    }
}

// ============================================================
// 2. BUILDER
// ============================================================

public interface ILoanReceiptBuilderExercise
{
    LoanReceipt Build(
        Guid loanId,
        Guid resourceId,
        DateTimeOffset dueAt);
}

public sealed class LoanReceiptBuilderExercise
    : ILoanReceiptBuilderExercise
{
    public LoanReceipt Build(
        Guid loanId,
        Guid resourceId,
        DateTimeOffset dueAt)
    {
        if (loanId == Guid.Empty)
        {
            throw new ArgumentException(
                "El identificador del préstamo es obligatorio.",
                nameof(loanId));
        }

        if (resourceId == Guid.Empty)
        {
            throw new ArgumentException(
                "El identificador del recurso es obligatorio.",
                nameof(resourceId));
        }

        return new LoanReceipt(
            loanId,
            resourceId,
            dueAt);
    }
}

// ============================================================
// 3. ADAPTER
// ============================================================

public sealed record LegacyCatalogRecord(
    string InventoryCode,
    string DisplayName,
    string Category);

public sealed record CatalogImport(
    Guid Id,
    string InventoryCode,
    string Name,
    string Category);

public interface ILegacyCatalogAdapterExercise
{
    CatalogImport Adapt(LegacyCatalogRecord record);
}

public sealed class LegacyCatalogAdapterExercise
    : ILegacyCatalogAdapterExercise
{
    public CatalogImport Adapt(LegacyCatalogRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);

        if (string.IsNullOrWhiteSpace(record.InventoryCode))
        {
            throw new ArgumentException(
                "El código de inventario es obligatorio.",
                nameof(record));
        }

        if (string.IsNullOrWhiteSpace(record.DisplayName))
        {
            throw new ArgumentException(
                "El nombre del recurso es obligatorio.",
                nameof(record));
        }

        if (string.IsNullOrWhiteSpace(record.Category))
        {
            throw new ArgumentException(
                "La categoría es obligatoria.",
                nameof(record));
        }

        return new CatalogImport(
            Guid.NewGuid(),
            record.InventoryCode,
            record.DisplayName,
            record.Category);
    }
}

// ============================================================
// MODELOS Y CONTRATOS PARA DECORATOR
// ============================================================

public sealed record LoanReceipt(
    Guid LoanId,
    Guid ResourceId,
    DateTimeOffset DueAt);

public sealed record LoanNotice(
    Guid LoanId,
    string Recipient);

public interface ILoanNoticeSender
{
    Task SendAsync(
        LoanNotice notice,
        CancellationToken cancellationToken);
}

public interface ILoanNoticeAudit
{
    Task RecordAsync(
        LoanNotice notice,
        CancellationToken cancellationToken);
}

// ============================================================
// 4. DECORATOR
// ============================================================

public sealed class LoanNoticeDecoratorExercise(
    ILoanNoticeSender sender,
    ILoanNoticeAudit audit) : ILoanNoticeSender
{
    public async Task SendAsync(
        LoanNotice notice,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(notice);

        // Ejecuta el envío mediante el componente original.
        await sender.SendAsync(
            notice,
            cancellationToken);

        // Registra la operación en el servicio de auditoría.
        await audit.RecordAsync(
            notice,
            cancellationToken);
    }
}

// ============================================================
// MODELO Y CONTRATO PARA FACADE
// ============================================================

public sealed record MemberOverview(
    Guid UserId,
    string UserName,
    IReadOnlyList<LoanReceipt> ActiveLoans);

public interface IMemberOverviewFacadeExercise
{
    Task<MemberOverview> GetAsync(
        Guid userId,
        CancellationToken cancellationToken);
}

// ============================================================
// 5. FACADE
// ============================================================

public sealed class MemberOverviewFacadeExercise(
    ILibraryUserRepository users,
    ILoanRepository loans,
    ILibraryResourceRepository resources)
    : IMemberOverviewFacadeExercise
{
    public async Task<MemberOverview> GetAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "El identificador del usuario es obligatorio.",
                nameof(userId));
        }

        // 1. Consultar el usuario.
        var user = await users.FindAsync(
            userId,
            cancellationToken);

        if (user is null)
        {
            throw new ApplicationNotFoundException(
                $"No se encontró el usuario {userId}.");
        }

        // 2. Obtener los préstamos registrados.
        var allLoans = await loans.ListAsync(
            cancellationToken);

        var activeLoans = new List<LoanReceipt>();

        // 3. Seleccionar los préstamos activos del usuario
        // y comprobar que su recurso exista.
        foreach (var loan in allLoans)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (loan.UserId != userId ||
                loan.Status != LoanStatus.Active)
            {
                continue;
            }

            var resource = await resources.FindAsync(
                loan.ResourceId,
                cancellationToken);

            if (resource is null)
            {
                throw new ApplicationNotFoundException(
                    $"No se encontró el recurso {loan.ResourceId} " +
                    $"del préstamo {loan.Id}.");
            }

            // 4. Construir el recibo del préstamo activo.
            activeLoans.Add(
                new LoanReceipt(
                    loan.Id,
                    resource.Id,
                    loan.DueAt));
        }

        // 5. Devolver el resumen del miembro.
        return new MemberOverview(
            user.Id,
            user.Name,
            activeLoans.AsReadOnly());
    }
}