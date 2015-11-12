using Chalkable.BusinessLogic.Model.Reports;
using Chalkable.BusinessLogic.Services.Master;
using Chalkable.BusinessLogic.Services.School;

namespace Chalkable.BusinessLogic.Services.Reporting
{
    interface IReportHandler<in TSettings>
    {
        object PrepareDataSource(TSettings settings, ReportingFormat format, IServiceLocatorSchool serviceLocator, IServiceLocatorMaster masterLocator);
        string GetReportDefinitionFile();
    }

    //public static class ReportParametersExtensions
    //{
    //    public static DateTime? GetDateTime(this Dictionary<string, string> parameters, string name)
    //    {
    //        if (!parameters.ContainsKey(name)) return null;
    //        string d = parameters[name];
    //        DateTime dt;
    //        if (DateTime.TryParse(d, out dt))
    //            return dt;
    //        return null;
    //    }

    //    public static int? GetInt(this Dictionary<string, string> parameters, string name)
    //    {
    //        if (!parameters.ContainsKey(name)) return null;
    //        string d = parameters[name];
    //        int a;
    //        if (int.TryParse(d, out a))
    //            return a;
    //        return null;
    //    }
    //}
}
