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
        Xml = 5,
        Json = 6,
        Word = 7
    }
    public static class ReportingFormatExtension
    {
        private const string EXT_XLS = "xls";
        private const string EXT_PDF = "pdf";
        private const string EXT_TIFF = "tiff";
        private const string EXT_CSV = "csv";
        private const string EXT_XML = "xml";
        private const string EXT_HTML = "html";
        private const string EXT_JSON = "json";
        private const string EXT_DOC = "doc";
        
        
        private const string EXCEL = "Excel";
        private const string PDF = "Pdf";
        private const string WORD = "Word";
        private const string CSV = "Csv";
        private const string JSON = "JSON";


        public static string AsString(this ReportingFormat format)
        {
            if (format == ReportingFormat.Excel)
                return EXCEL;
            if (format == ReportingFormat.Pdf)
                return PDF;
            if (format == ReportingFormat.Word)
                return WORD;
            if (format == ReportingFormat.Csv)
                return CSV;
            if (format == ReportingFormat.Json)
                return JSON;
            throw new Exception(ChlkResources.ERR_INVALID_REPORT_FORMAT);
        }


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
                case ReportingFormat.Json: return EXT_JSON;
                case ReportingFormat.Word: return EXT_DOC;
                default:
                    throw new Exception(ChlkResources.ERR_INVALID_REPORT_FORMAT);
            }
        }
    }
}
