using GIPractice.Api.Models;
using GIPractice.Wpf.Backend;
using GIPractice.Wpf.Backend.Queries.Patients;

namespace GIPractice.Wpf.ViewModels.Patients;

public sealed class PatientDetailsViewModel : SingleViewModel<PatientDto>
{
    private readonly int _id;

    /// <param name="id">Existing patient id, or 0 for new patient.</param>
    public PatientDetailsViewModel(IDatabase database, int id)
        : base(database)
    {
        _id = id;
        IsNew = id == 0;

        if (IsNew)
        {
            Item = new PatientDto(); // or some default ctor
        }
    }

    protected override IBackendQuery<PatientDto> CreateLoadQuery()
    {
        if (_id <= 0)
            // For a new patient we don't load anything
            return new GetNewPatientQueryStub();

        return new GetPatientQuery(_id);
    }

    protected override IBackendQuery<PatientDto> CreateSaveQuery(PatientDto item)
        => new SavePatientQuery(item);

    protected override bool DetermineIsNew(PatientDto item)
        => item.Id == 0;

    /// <summary>
    /// Small stub to satisfy the base class when creating a new entity (no load).
    /// </summary>
    private sealed class GetNewPatientQueryStub : IBackendQuery<PatientDto>
    {
        public Task<PatientDto> ExecuteAsync(BackendContext context, CancellationToken cancellationToken)
        {
            // Just return an empty DTO; no API call.
            return Task.FromResult(new PatientDto());
        }
    }
}
