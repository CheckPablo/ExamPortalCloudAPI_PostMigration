﻿using ExamPortalApp.Contracts.Data.Dtos.Custom;
using ExamPortalApp.Contracts.Data.Entities;
using ExamPortalApp.Contracts.Data.Repositories;
using ExamPortalApp.Contracts.Data.Repositories.Generic;
using ExamPortalApp.Infrastructure.Constants;
using ExamPortalApp.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ExamPortalApp.Infrastructure.Data.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly IRepository _repository;
        private readonly DecodedUser? _user;
        private List<Subject> request;

        public SubjectRepository(IRepository repository, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _user = accessor.GetLoggedInUser();
        }

        public async Task<Subject> AddAsync(Subject entity)
        {

            var subjectExists = await _repository.AnyAsync<Subject>(x => x.SectorId == entity.SectorId && x.Code == entity.Code);
            if (subjectExists)
            {
                //throw new Exception(ErrorMessages.SubjectEntryChecks.SubjectExists);
                throw new InvalidSubjectEntryException();
            }
            return await _repository.AddAsync(entity, true);
        }
        public async Task<Subject> UpdateLinkToAllAsync(Subject entity)
        {
            await UpdateAsync(entity); 
            /*var parameters = new Dictionary<string, object>();

            if (entity.ModifiedBy == null)
            {
                entity.ModifiedBy = _user.Id;
            }

            parameters.Add(StoredProcedures.Params.SubjectId, entity.Id);
            parameters.Add(StoredProcedures.Params.SectorId, entity.SectorId);
            parameters.Add(StoredProcedures.Params.Code, entity.Code);
            parameters.Add(StoredProcedures.Params.Description, entity.Description);
            parameters.Add(StoredProcedures.Params.ModifiedBy, _user.Id);

            var result = await _repository.ExecuteStoredProcAsync<UserCenter>(StoredProcedures.SubjectMaintenance_InsUpd, parameters);*/

            var studentsToLink = await _repository.GetWhereAsync<Student>(x => x.GradeId == entity.SectorId);

            var subjectLastEntry = await _repository.GetWhereAsync<Subject>(x => x.SectorId == entity.SectorId && x.Code == entity.Code);
            foreach (var student in studentsToLink)
            {
                var studentCount = await _repository.GetWhereAsync<StudentSubject>(x => x.StudentId == student.Id && x.SubjectId == subjectLastEntry.First().Id);
                if (studentCount.Count() == 0)
                {
                    var studentSub = new StudentSubject()
                    {
                        StudentId = student.Id,
                        SubjectId = subjectLastEntry.First().Id,
                        OldSubjectId = student.Id,
                    };
                    await _repository.AddAsync(studentSub, true);
                }
            }
            return (entity);
        }
        
            public async Task<Subject> AddLinkToAllAsync(Subject entity)
        {

            var subjectExists = await _repository.AnyAsync<Subject>(x => x.SectorId == entity.SectorId && x.Code == entity.Code);
            if (subjectExists)
            {
                throw new Exception(ErrorMessages.SubjectEntryChecks.SubjectExists);
            }

            //var studentsToLink = await _repository.AnyAsync<Subject>(x => x.SectorId == entity.SectorId && x.Code == entity.Code);
            //var studentsToLink = _repository.Where(x => x.Subject != null).Select(x => x.Subject);
            var parameters = new Dictionary<string, object>();

            if(entity.ModifiedBy == null)
            {
                entity.ModifiedBy = _user.Id; 
            }

            parameters.Add(StoredProcedures.Params.SubjectId, entity.Id);
            parameters.Add(StoredProcedures.Params.SectorId, entity.SectorId);
            parameters.Add(StoredProcedures.Params.Code, entity.Code);
            parameters.Add(StoredProcedures.Params.Description, entity.Description);
            parameters.Add(StoredProcedures.Params.ModifiedBy, _user.Id);

            var result = await _repository.ExecuteStoredProcAsync<UserCenter>(StoredProcedures.SubjectMaintenance_InsUpd, parameters);

            var studentsToLink = await _repository.GetWhereAsync<Student>(x => x.GradeId == entity.SectorId);

            var subjectLastEntry = await _repository.GetWhereAsync<Subject>(x => x.SectorId == entity.SectorId && x.Code == entity.Code);
            foreach (var student in studentsToLink )
            {
                var studentCount = await _repository.GetWhereAsync<StudentSubject>(x => x.StudentId == student.Id && x.SubjectId == subjectLastEntry.First().Id);
                if(studentCount.Count() == 0 )
                {
                    var studentSub = new StudentSubject()
                    {
                        StudentId = student.Id,
                        SubjectId = subjectLastEntry.First().Id,
                        OldSubjectId = student.Id,
                    };
                    await _repository.AddAsync(studentSub, true);
                }
            }
            return (entity);
                //await _repository.AddAsync(entity, true);
        }

        public async Task<int> DeleteAsync(int id)
        {
            await _repository.DeleteAsync<Subject>(id);

            return await _repository.CompleteAsync();
        }

        public async Task<IEnumerable<Subject>> GetAllAsync()
        {
            // return await _repository.GetAllAsync<Subject>();   
            request = await _repository.GetQueryable<Subject>()

                             .Join(_repository.GetQueryable<Grade>(), t => t.SectorId, s =>
                              s.Id, (t, s) => new { Subject = t, Grade = s })
                             .Where(s => s.Grade.CenterId == _user.CenterId)
                             .Select(x => new Subject
                             {
                                 Id = x.Subject.Id,
                                 Code = x.Subject.Code,
                                 Description = x.Subject.Description,
                                 ModifiedBy = x.Subject.ModifiedBy,
                                 ModifiedDate = x.Subject.ModifiedDate,
                                 SectorId = x.Subject.SectorId,
                                 SubjectGrade = x.Grade.Code,
                                 //SubjectGrade = "check",
                             }).ToListAsync();         
            return request.OrderBy(x => x.SubjectGrade).ThenBy(x => x.Description);

        }

        public async Task<Subject> GetAsync(int id)
        {
            var entity = await _repository.GetByIdAsync<Subject>(id);

            if (entity == null) throw new KeyNotFoundException(typeof(Subject).Name);

            return entity;
        }

        public async Task<IEnumerable<Subject>> GetByGradeAsync(int id)
        {
            var subjects = await _repository.GetWhereAsync<Subject>(x => x.SectorId == id);

            return subjects.OrderBy(x => x.Description);
            //return subjects.Distinct().OrderBy(x => x.Description);
        }

        public async Task<IEnumerable<Subject>> GetByStudentAsync(int id)
        {
            var studentSubjects = await _repository.GetWhereAsync<StudentSubject>(x => x.StudentId == id && x.Subject != null, x => x.Subject);
            var subjects = studentSubjects.Where(x => x.Subject != null).Select(x => x.Subject);

            if (subjects == null) return new List<Subject>();

            return subjects.OrderBy(x => x!.Description)!;
        }

        public async Task<Subject> UpdateAsync(Subject entity)
        {
            var subject = await _repository.GetByIdAsync<Subject>(entity.Id);

            if (subject == null)
            {
                //throw new NotImplementedException();
                 throw new InvalidSubjectEntryException();
            }
            else
            {
                subject.Code = entity.Code;
                subject.Description = entity.Description;
                return await _repository.UpdateAsync(subject, true);
            }
        }
    }
}
