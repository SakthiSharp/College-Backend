using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagement.Entities
{
    [Table("COLLEGE")]
    public class CollegeEntity
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public virtual ICollection<DepartmentEntity> DepartmentEntities { get; set; }

    }
}
