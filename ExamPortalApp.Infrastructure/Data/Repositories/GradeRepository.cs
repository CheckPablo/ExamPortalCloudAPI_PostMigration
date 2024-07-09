using ExamPortalApp.Contracts.Data.Dtos.Custom;
using ExamPortalApp.Contracts.Data.Entities;
using ExamPortalApp.Contracts.Data.Repositories;
using ExamPortalApp.Contracts.Data.Repositories.Generic;
using ExamPortalApp.Infrastructure.Constants;
using ExamPortalApp.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;

namespace ExamPortalApp.Infrastructure.Data.Repositories
{
    public class GradeRepository : IGradeRepository
    {
        private readonly IRepository _repository;
        private readonly DecodedUser? _user;

        public GradeRepository(IRepository repository, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _user = accessor.GetLoggedInUser();
        }

    /*     public async Task<Grade> AddAsync(Grade entity)
        {
            if (_user == null)
            {
                throw new Exception(ErrorMessages.Auth.Unauthorised);
            }
            entity.CenterId = _user.CenterId;
            var gradeExists = await _repository.AnyAsync<Grade>(x => x.Code == entity.Code && x.CenterId==entity.CenterId && !entity.IsDeleted);
        // var gradeExists = await _repository.AnyAsync<Grade>(x => x.Code == entity.Code && x.CenterId==entity.CenterId && !entity.IsDeleted); //soft delete is 1 therefore o = grade exists 
            if (gradeExists)
            {
                throw new Exception(ErrorMessages.GradeEntryChecks.GradeExists);
            }
            return await _repository.AddAsync(entity, true);
        } */

        public async Task<Grade> AddAsync(Grade entity)
        {
            // 0 is true in SQL 
            if (_user == null)
            {
                throw new Exception(ErrorMessages.Auth.Unauthorised);
            }
            entity.CenterId = _user.CenterId;
            //var gradeExists = await _repository.AnyAsync<Grade>(x => x.Code == entity.Code && x.CenterId==entity.CenterId);
            var gradeExists = await _repository.AnyAsync<Grade>(x => x.Code == entity.Code && x.CenterId==entity.CenterId);
             var deleteStatus =  await _repository.GetFirstOrDefaultGradeAsync<Grade>(x => x.Code == entity.Code && x.CenterId==entity.CenterId);
            //var gradeExists = await _repository.GetWhereAsync<Grade>(x => x.Code == entity.Code && x.CenterId==entity.CenterId);
            //var gradeExists = await _repository.GetFirstOrDefaultAsync<Grade>(x => x.Code == entity.Code && x.CenterId==entity.CenterId);
         //var gradeExists = await _repository.AnyAsync<Grade>(x => x.Code == entity.Code && x.CenterId==entity.CenterId && !entity.IsDeleted); //soft delete is 1 therefore o = grade exists 
            if (gradeExists)
            {
               
                if(deleteStatus != null && deleteStatus.IsDeleted == false)
                {
                 //throw new Exception(ErrorMessages.GradeEntryChecks.GradeExists);
                 throw new InvalidGradeEntryException();  
                }
              
            }
              if(deleteStatus != null && deleteStatus.IsDeleted == true){
                  deleteStatus.IsDeleted = false; 
                 return await _repository.UpdateAsync(deleteStatus, true); 
                }
                
            else{
                return await _repository.AddAsync(entity, true);
            }
        }


        public async Task<int> DeleteAsync(int id)
        {
            
            await _repository.DeleteAsync<Grade>(id);
            await DeleteGradeSubjectAsync(id);
            await DeleteStudentGrade(id);
            return await _repository.CompleteAsync();
        }

        public async Task<int> DeleteGradeSubjectAsync(int id)
        {
            var gradeSubject = await _repository.GetFirstOrDefaultAsync<Subject>(x => x.SectorId == id);
            if(gradeSubject != null)
            {
            await _repository.DeleteAsync<Subject>(gradeSubject.SectorId);

            return await _repository.CompleteAsync();
        }
        else{
            return 0; 
        }
        }

         public async Task<int> DeleteStudentGrade(int id)
        {
            var studentGrade = await _repository.GetFirstOrDefaultAsync<Student>(x => x.GradeId == id);
            if(studentGrade != null){
                await _repository.DeleteAsync<Student>(studentGrade.GradeId?? 0);

                return await _repository.CompleteAsync();
            }
            else{
                return 0; 
            }
        }

        public async Task<IEnumerable<Grade>> GetAllAsync() 
        {
            if (_user == null)
            {
                throw new Exception(ErrorMessages.Auth.Unauthorised);
            }
            var grades = await _repository.GetWhereAsync<Grade>(x => x.CenterId == _user.CenterId);
            return grades.OrderBy(x => x.Code);
            //return grades.Distinct().OrderBy(x => x.Code);
        }


        public async Task<IEnumerable<Grade>> GetAllByCenterIdAsync(int? id)
        {
            if (_user == null)
            {
                throw new Exception(ErrorMessages.Auth.Unauthorised);
            }
            var grades = await _repository.GetWhereAsync<Grade>(x => x.CenterId == id);
            return grades.OrderBy(x => x.Code);
            //return grades.Distinct().OrderBy(x => x.Code);
        }

        //public Task<IEnumerable<Grade>> GetAllByCenterIdAsync(int id)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<Grade> GetAsync(int id)
        {
            var entity = await _repository.GetByIdAsync<Grade>(id);

            if (entity == null) throw new EntityNotFoundException<Grade>(id);

            return entity;
        }

        public async Task UpdateGradeAsync(Grade entity)
        {
            if (entity == null)
            {
                throw new Exception(ErrorMessages.Auth.Unauthorised);
            }
            //await Task.Run
            //await Task.Run(Task); 
            //var grade = await _repository.GetWhereAsync<Grade>(x => x.Id == entity.Id);
        }

        public async Task<Grade> UpdateAsync(Grade entity)
        {
            var gradeToUpdate = await _repository.GetByIdAsync<Grade>(entity.Id);

            if (gradeToUpdate == null || gradeToUpdate.IsDeleted)
            {
                throw new NotImplementedException();
            }
            else
            {
                gradeToUpdate.Code = entity.Code;
                gradeToUpdate.Description = entity.Description;

                return await _repository.UpdateAsync(gradeToUpdate, true);
            }
        }

    
        //public Task<GradeDto> GetAllByCenterIdAsync(int id)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
