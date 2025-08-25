using System.Runtime.Intrinsics.Arm;
using System.Threading.Tasks;
using CollegeManagement.Dto;
using CollegeManagement.Entities;
using CollegeManagement.Migrations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.Controllers
{
    [ApiController]
    [Route("api/college")]
    public class CollegeController
    {
        private readonly CollegeDbContext _dbContext;

        public CollegeController(CollegeDbContext context)
        {
            _dbContext = context;
        }

        [HttpGet]
        [Route("get-college")]
        public async Task<CollegeDto> GetCollegeById(Guid id)
        {
            var collgeDetails = new CollegeDto();
            var college = await _dbContext.CollegeEntities.FirstOrDefaultAsync(c => c.Id == id);
            if (college != null)
            {
                collgeDetails.Id = college.Id;
                collgeDetails.Address = college.Address;
                collgeDetails.Name = college.Name;
                collgeDetails.Description = college.Description;
                var departments = await GetDepartMents(college.Id) ?? [];
                collgeDetails.Departments = departments;
            }
            return collgeDetails;
        }

        [HttpGet]
        [Route("get-all-college")]
        public async Task<List<CollegeDto>> GetCollegeById()
        {
            var collegeEntities = await _dbContext.CollegeEntities.ToListAsync();

            var college = new List<CollegeDto>();

            foreach (var cl in collegeEntities)
            {
                var dto = new CollegeDto
                {
                    Id = cl.Id,
                    Address = cl.Address,
                    Name = cl.Name,
                    Description = cl.Description,
                    Departments = await GetDepartMents(cl.Id)
                };
                college.Add(dto);
            }

            return college;
        }

        [HttpPost]
        [Route("upsert-college")]
        public async Task<CollegeDto> UpsertCollegeDetails(CollegeDto collegeDetails)
        {

            if (collegeDetails.Id == Guid.Empty)
            {
                CollegeEntity college = new CollegeEntity
                {
                    Id = Guid.NewGuid(),
                    Address = collegeDetails.Address,
                    Name = collegeDetails.Name,
                    Description = collegeDetails.Description,
                };
                collegeDetails.Id = college.Id;
                _dbContext.CollegeEntities.Add(college);
                await _dbContext.SaveChangesAsync();
                await UpsertDepartMent(collegeDetails);
            }
            else
            {
                var college = await _dbContext.CollegeEntities.FirstOrDefaultAsync(cl => cl.Id == collegeDetails.Id);
                if (college != null)
                {
                    college.Name = collegeDetails.Name;
                    college.Address = collegeDetails.Address;
                    college.Description = collegeDetails.Description;
                    await _dbContext.SaveChangesAsync();
                    await UpsertDepartMent(collegeDetails);
                    
                }
            }
            return collegeDetails;
        }

        [HttpGet]
        [Route("get-departments")]
        public async Task<List<DepartmentDto>> GetDepartMents(Guid collegId)
        {
            var departMents = await _dbContext.DepartmentEntities
                                .Where(dp => dp.CollegeId == collegId)
                                .Select(dp => new DepartmentDto
                                {
                                    Id = dp.Id,
                                    Name = dp.Name,
                                    CollegeId = dp.CollegeId,
                                    Description = dp.Description,
                                    Percentage = dp.Percentage,
                                })
                                .ToListAsync();
            return departMents;
        }

        [HttpDelete]
        [Route("delete-college")]
        public async Task DeleteCollege(Guid collegeId)
        {
            var college = await _dbContext.CollegeEntities.FindAsync(collegeId);

            if (college != null)
            {
                var departMents = (await _dbContext.DepartmentEntities.ToListAsync()).Where(dt => dt.CollegeId == college.Id).ToList();
                foreach (var dt in departMents)
                {
                    await DeleteDepartment(dt.Id);
                }
                _dbContext.CollegeEntities.Remove(college);
                await _dbContext.SaveChangesAsync();
                
            }
        }

        [HttpDelete]
        [Route("delete-department")]
        public async Task DeleteDepartment(Guid departmentId)
        {
            var department = await _dbContext.DepartmentEntities.FindAsync(departmentId);

            if (department != null)
            {

                _dbContext.DepartmentEntities.Remove(department);
                await _dbContext.SaveChangesAsync();
            }
        }


        private async Task UpsertDepartMent(CollegeDto college)
        {
            var allDepartMents = await _dbContext.DepartmentEntities
                                .Where(dp => dp.CollegeId == college.Id)
                                .ToListAsync();
            foreach (DepartmentDto department in college.Departments)
            {
                if (department.Id == Guid.Empty)
                {
                    DepartmentEntity dt = new()
                    {
                        Id = Guid.NewGuid(),
                        Name = department.Name,
                        Description = department.Description,
                        Percentage = department.Percentage,
                        CollegeId = college.Id,
                    };
                    department.Id = dt.Id;
                    _dbContext.DepartmentEntities.Add(dt);
                    await _dbContext.SaveChangesAsync(true);
                }
                else
                {
                    var dt = await _dbContext.DepartmentEntities.FirstOrDefaultAsync(dp => dp.Id == department.Id);
                    if (dt != null)
                    {
                        dt.Name = department.Name;
                        dt.Description = department.Description;
                        dt.Percentage = department.Percentage;
                        await _dbContext.SaveChangesAsync(true);
                    }
                }
            }
            var incomingIds = college.Departments.Select(d => d.Id).ToList();
            var toDelete = allDepartMents.Where(dp => !incomingIds.Contains(dp.Id)).ToList();

            if (toDelete.Any())
            {
                _dbContext.DepartmentEntities.RemoveRange(toDelete);
            }

            await _dbContext.SaveChangesAsync();

        }
    }
}
