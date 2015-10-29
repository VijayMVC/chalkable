using System;
using Chalkable.Common;

namespace Chalkable.BusinessLogic.Model.Reports
{
    public class BaseReportInputModel
    {
        public virtual int IdToPrint { get; set; }

        public ReportingFormat FormatTyped
        {
            get { return (ReportingFormat) Format; }
            set { Format = (int) value; }
        }

        public virtual int Format { get; set; }
        public virtual int GradingPeriodId { get; set; }
        public virtual int ClassId { get; set; }
        public IntList StudentIds { get; set; }
    }
    
    public enum ReportingFormat
    {
        Pdf = 0,
        Csv = 1,
        Excel = 2,
        Html = 3,
        Tiff = 4,
        Xml = 5
    }
    public static class ReportingFormatExtension
    {
        private const string EXT_XLS = "xls";
        private const string EXT_PDF = "pdf";
        private const string EXT_TIFF = "tiff";
        private const string EXT_CSV = "csv";
        private const string EXT_XML = "xml";
        private const string EXT_HTML = "html";

        public static string AsFileExtension(this ReportingFormat format)
        {
            switch (format)
            {
                case ReportingFormat.Pdf: return EXT_PDF;
                case ReportingFormat.Csv: return EXT_CSV;
                case ReportingFormat.Excel: return EXT_XLS;
                case ReportingFormat.Tiff: return EXT_TIFF;
                case ReportingFormat.Html: return EXT_HTML;
                case ReportingFormat.Xml: return EXT_XML;
                default:
                    throw new Exception(ChlkResources.ERR_INVALID_REPORT_FORMAT);
            }
        }
    }
}
