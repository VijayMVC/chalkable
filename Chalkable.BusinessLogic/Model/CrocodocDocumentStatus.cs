using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chalkable.BusinessLogic.Services;

namespace Chalkable.BusinessLogic.Model
{
    public enum DocumentStatus
    {
        Queued,
        Processing,
        Done,
        Error
    }

    public class CrocodocDocumentStatus
    {
        public string Uuid { get; set; }
        public DocumentStatus DocumentStatus { get; set; }

        public static CrocodocDocumentStatus Create(DocumentStatusResponce reponce)
        {
            return new CrocodocDocumentStatus
            {
                Uuid = reponce.Uuid,
                DocumentStatus = StatusMapper[reponce.Status]
            };
        }

        private static readonly IDictionary<string, DocumentStatus> StatusMapper = new Dictionary<string, DocumentStatus>
        {
            ["DONE"] = DocumentStatus.Done,
            ["PROCESSING"] = DocumentStatus.Processing,
            ["ERROR"] = DocumentStatus.Error,
            ["QUEUED"] = DocumentStatus.Queued
        };
    }
}
