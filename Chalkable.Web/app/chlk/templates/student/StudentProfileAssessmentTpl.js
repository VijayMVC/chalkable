REQUIRE('chlk.templates.student.StudentProfileSummaryTpl');
REQUIRE('chlk.models.student.StudentProfileAssessmentViewData');

NAMESPACE('chlk.templates.student', function () {

    /** @class chlk.templates.student.StudentProfileAssessmentTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/student/StudentProfileAssessmentPage.jade')],
        [ria.templates.ModelBind(chlk.models.student.StudentProfileAssessmentViewData)],
        'StudentProfileAssessmentTpl', EXTENDS(chlk.templates.student.StudentProfileSummaryTpl), [

        	[ria.templates.ModelPropertyBind],
        	chlk.models.apps.ExternalAttachAppViewData, 'attachAppViewData'
         
        ]);
});