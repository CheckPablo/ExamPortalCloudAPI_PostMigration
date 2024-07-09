using ExamPortalApp.Contracts.Data.Repositories.Generic;
using ExamPortalApp.Infrastructure.Constants;

namespace ExamPortalApp.Infrastructure;

public class PeopleObject(int centerNo, int regionID, string name, string surname, string idNumber, string studentNo, string batch, string email, string cellPhone, IRepository repository)
{
    private readonly IRepository _repository = repository;

    //private readonly IRepository? _repository;
    public int CenterNo { get; set; } = centerNo;
    public int RegionID { get; set; } = regionID;
    public string Name { get; set; } = name;
    public string Surname { get; set; } = surname;
    public string IDNumber { get; set; } = idNumber;
    public string StudentNo { get; set; } = studentNo;
    public string Batch { get; set; } = batch;
    public string? Email { get; set; } = email;
    public string? CellPhone { get; set; } = cellPhone;

    public async Task <bool> ImportToBuffer()
        {
        
            var parameters = new Dictionary<string, object>
            {
                { StoredProcedures.Params.Batch,  Batch },
                { StoredProcedures.Params.CenterNo, CenterNo },
                { StoredProcedures.Params.RegionID, RegionID },
                { StoredProcedures.Params.Name, Name },
                { StoredProcedures.Params.Surname, Surname },
                { StoredProcedures.Params.IDNumber, IDNumber },
                { StoredProcedures.Params.StudentNo, StudentNo},
                { StoredProcedures.Params.Email, Email ?? ""},
                { StoredProcedures.Params.CellPhone, CellPhone  ?? ""},
            
            };
            
            var result = await _repository.ExecuteStoredProcedureAsync<object>(StoredProcedures.ImportPeople, parameters).ConfigureAwait(true);
            return result is not null;
        }

    }

