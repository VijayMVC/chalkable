using System;
namespace Chalkable.StiConnector.SyncModel
{
    public class Document
    {
        public int DocumentID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Extension { get; set; }
        public int FileSizeInKB { get; set; }
        public string MIMEType { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }
        public Guid RowVersion { get; set; }
        public byte[] Data { get; set; }
        public Guid RowIdentifier { get; set; }
    
    }
}
