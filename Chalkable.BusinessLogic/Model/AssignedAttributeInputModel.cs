namespace Chalkable.BusinessLogic.Model
{
    public class AssignedAttributeInputModel
    {
        public int Id { get; set; }
        public int AnnouncementId { get; set; }
        public int? AttachmentId { get; set; }
        public int AttributeTypeId { get; set; }

        public string Name { get; set; }
        public string Text { get; set; }
        public bool VisibleForStudents { get; set; }

        //TODO: remove those fields later
        //public int? SisActivityAssignedAttributeId { get; set; }
        //public int? SisActivityId { get; set; }
    }
    
}
