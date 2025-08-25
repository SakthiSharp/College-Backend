using CollegeManagement.Entities;

namespace CollegeManagement.Dto
{
    public class DepartmentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Percentage { get; set; }
        public Guid CollegeId { get; set; }
    }
}
