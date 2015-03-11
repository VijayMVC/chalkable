REQUIRE('chlk.templates.reports.GradeBookReportTpl');
REQUIRE('chlk.models.reports.GradeBookReportViewData');

NAMESPACE('chlk.templates.reports', function () {

    /** @class chlk.templates.reports.WorksheetReportGridTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/reports/WorksheetReportGrid.jade')],
        [ria.templates.ModelBind(chlk.models.reports.GradeBookReportViewData)],
        'WorksheetReportGridTpl', EXTENDS(chlk.templates.reports.GradeBookReportTpl), [
            String, function getAnnouncementIds(){
                var res = [];
                this.getAnnouncements().forEach(function(item){
                    res.push(item.getId().valueOf());
                });
                return res.join(',')
            }
        ])
});