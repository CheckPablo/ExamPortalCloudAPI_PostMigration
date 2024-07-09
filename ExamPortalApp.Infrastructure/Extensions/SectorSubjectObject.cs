namespace ExamPortalApp.Infrastructure;
using ExamPortalApp.Contracts.Data.Repositories.Generic;
using ExamPortalApp.Infrastructure.Constants;
public class SectorSubjectsObject(string sectorCode, string sector, string subjectCode, string _Subject, string _StudentNo, string _Batch, IRepository repository)
{
    private readonly IRepository _repository = repository;
    public string SectorCode = sectorCode;
    public string Sector = sector;
    public string SubjectCode = subjectCode;
    public string Subject = _Subject;
    public string StudentNo = _StudentNo;
    public string Batch = _Batch;

    public bool ImportToBuffer()
    {

        var parameters = new Dictionary<string, object>
            {
                { StoredProcedures.Params.Batch, this.Batch },
                { StoredProcedures.Params.Sector, this.Sector },
                { StoredProcedures.Params.SectorCode, this.SectorCode },
                { StoredProcedures.Params.SubjectCode, this.SubjectCode },
                { StoredProcedures.Params.Subject, this.Subject },
                { StoredProcedures.Params.StudentNo, this.StudentNo }
            };
        var result = _repository.ExecuteStoredProcAsync<object>(StoredProcedures.ImportSubjectSectors, parameters);
        return result is not null;
    }
}
