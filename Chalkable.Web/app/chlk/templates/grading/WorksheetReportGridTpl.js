REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.grading.GradeBookReportViewData');

NAMESPACE('chlk.templates.grading', function () {

    /** @class chlk.templates.grading.WorksheetReportGridTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/WorksheetReportGrid.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradeBookReportViewData)],
        'WorksheetReportGridTpl', EXTENDS(chlk.templates.grading.GradeBookReportTpl), [
            String, function getAnnouncementIds(){
                var res = [];
                this.getAnnouncements().forEach(function(item){
                    res.push(item.getId().valueOf());
                });
                return res.join(',')
            }
        ])
});