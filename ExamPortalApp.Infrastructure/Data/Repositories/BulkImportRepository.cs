using ExamPortalApp.Contracts.Data.Entities;
using ExamPortalApp.Contracts.Data.Repositories;
using ExamPortalApp.Contracts.Data.Repositories.Generic;
using ExamPortalApp.Infrastructure.Constants;
using Syncfusion.XlsIO;
namespace ExamPortalApp.Infrastructure.Data.Repositories
{
    public class BulkImportRepository(IRepository repository) : IBulkImportRepository
    {
        private readonly IRepository _repository = repository;

        public Task<StudentTest> AddAsync(StudentTest entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<StudentTest>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<StudentTest> GetAsync(int id)
        {
            throw new NotImplementedException();
        }


        public async Task<bool> ImportFile2(Stream stream, string batchGuid)
        {
            List<SectorSubjectsObject> lstSectorSubjects = [];
            using ExcelEngine excelEngine = new();
            IApplication application = excelEngine.Excel;

            application.DefaultVersion = ExcelVersion.Excel2016;

            IWorkbook workbook = excelEngine.Excel.Workbooks.Open(stream);

            IWorksheet worksheet = workbook.Worksheets[0];


            Dictionary<string, string> cells = new(){
                {"A1","SectorCode"},
                {"B1","Sector"},
                {"C1","SubjectCode"},
                {"D1","Subject"},
                {"F1","StudentNo"},
            };
            for (int i = worksheet.Columns[1].Count - 1; i >= 0; i--)
            {
                if (worksheet.Rows[i].IsBlank)
                {
                    worksheet.DeleteRow(i + 1);
                }
            }
            
            if(!CheckHeaders(worksheet, cells)) return false;

            for (int row = 2; row <= worksheet.Columns[1].Count; row++)
            {
                 var studentNo = "VST" + worksheet[row, 5].Value;
                //SectorSubjectsObject tempSectorSubjectsObject = new(worksheet[row, 1].Value, worksheet[row, 2].Value, worksheet[row, 3].Value, worksheet[row, 4].Value, studentNo, batchGuid, _repository);
                SectorSubjectsObject tempSectorSubjectsObject = new(worksheet[row, 1].Value, worksheet[row, 2].Value, worksheet[row, 3].Value, worksheet[row, 4].Value, worksheet[row, 5].Value, batchGuid, _repository);
                
                lstSectorSubjects.Add(tempSectorSubjectsObject);
            }

            foreach (var item in lstSectorSubjects)
            {
                item.ImportToBuffer();
            }

            if (await ImportToBufferLink())
            {
                //return true; 
                 //await Task.Delay(3000);
                 await BulkImportExamPortalCloud(batchGuid);
                 return true; 
            }
            else
            {
                return false;
            }

        }

        public async Task<bool>  BulkImportExamPortalCloud(string batchGuid)
        {
            //string  batchId = ""; 
            var parameters = new Dictionary<string, object>
            {
                { StoredProcedures.Params.BatchID, batchGuid }
            };

            var result = await _repository.ExecuteStoredProcedureAsync<object>(StoredProcedures.BulkImportExamPortalCloudEdited, parameters).ConfigureAwait(true);
            //return result is not null;
             if(result != null){
                return true; 
            }
            else{
                return false; 
            } 
        }

        public async Task<bool> ImportToBufferLink()
        {

            var parameters = new Dictionary<string, object>();
            string blankParam = "";
            parameters.Add(StoredProcedures.Params.BlankParam, blankParam);
            var result = _repository.ExecuteStoredProcedureAsync<object>(StoredProcedures.ImportLink, parameters);
            if(result is not null){
                return true; 
            }
            else{
                return false; 
            }
            //await Task.Delay(3000);
            //return await result is not null;
        }

        public Task<StudentTest> UpdateAsync(StudentTest entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ImportFile1(Stream PeopleFileStream, string batchGuid)
        {
            // string  batchGUID = "";
            List<PeopleObject> lstPeople = [];
            using ExcelEngine excelEngine = new();

            IApplication application = excelEngine.Excel;
            application.DefaultVersion = ExcelVersion.Excel2016;

            IWorkbook workbook = excelEngine.Excel.Workbooks.Open(PeopleFileStream);
            IWorksheet worksheet = workbook.Worksheets[0];
           /*  var centerNo = worksheet[row, 1].Value
            var centerNo =  */

            Dictionary<string, string> cells = new(){
                {"A1","CenterNo"},
                {"B1","RegionID"},
                {"C1","Name"},
                {"D1","Surname"},
                {"E1","IDNumber"},
                {"F1","StudentNo"},
            };
            for (int i = worksheet.Columns[1].Count - 1; i >= 0; i--)
            {
                if (worksheet.Rows[i].IsBlank)
                {
                    worksheet.DeleteRow(i + 1);
                }
            }

            if(!CheckHeaders(worksheet, cells)) return false;

            for (int row = 2; row <= worksheet.Columns[1].Count; row++)
            {
               /*  var centerNo = worksheet[row, 1].Value; 
                var centerNoRecord =  await _repository.GetFirstOrDefaultAsync<Center>(x => x.CenterNo == Convert.ToInt32(centerNo));
                var prefix = centerNoRecord.Prefix; */ 
                int RegionID = 1000;
                if (worksheet[row, 2].Value != "")
                {
                    RegionID = Convert.ToInt32(worksheet[row, 2].Value);
                }
                var studentNo  = "VST" + worksheet[row, 6].Value; 
                //PeopleObject tempPeopleObject = new(Convert.ToInt32(worksheet[row, 1].Value), RegionID, worksheet[row, 3].Value, worksheet[row, 4].Value, worksheet[row, 5].Value, studentNo, batchGuid, worksheet[row, 7].Value, worksheet[row, 8].Value, _repository);
                PeopleObject tempPeopleObject = new(Convert.ToInt32(worksheet[row, 1].Value), RegionID, worksheet[row, 3].Value, worksheet[row, 4].Value, worksheet[row, 5].Value, worksheet[row, 6].Value, batchGuid, worksheet[row, 7].Value, worksheet[row, 8].Value, _repository);

                lstPeople.Add(tempPeopleObject);
            }

            foreach (var item in lstPeople)
            {
                await item.ImportToBuffer().ConfigureAwait(false);
            }
            return true;
        }

        private static bool CheckHeaders(IWorksheet worksheet, Dictionary<string, string> cells)
        {
            //foreach (KeyValuePair<string, string> cell in cells)
            //{
                //if (!worksheet.Range[cell.Key].Text.Equals(cell.Value)) return false;
            //}
            return true;
        }

        public async Task<BulkImportPerson?> GetBatchID()
        {
            var batchGuidId = await _repository.ExecuteStoredProcAsync<BulkImportPerson>(StoredProcedures.GetBatchID_BulkImport, []);
            return batchGuidId.ToList().FirstOrDefault();
        }
    }
}
