using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagement.Entities
{
    [Table("DEPARTMENT")]
    public class DepartmentEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Percentage { get; set; } = 0;
        public virtual CollegeEntity CollegeReference { get; set; }
        public Guid CollegeId { get; set; }
    }
}
