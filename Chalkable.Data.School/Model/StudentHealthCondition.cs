namespace Chalkable.Data.School.Model
{
    public class StudentHealthCondition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsAlert { get; set; }
        public string MedicationType { get; set; }
        public string Treatment { get; set; }
    }
}
