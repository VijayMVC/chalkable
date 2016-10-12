REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.student.StudentProfileAssessmentTpl');

NAMESPACE('chlk.activities.student', function () {

    /** @class chlk.activities.student.StudentProfileSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.student.StudentProfileAssessmentTpl)],
        'StudentProfileAssessmentPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [[Object]],
            OVERRIDE, VOID, function onRefresh_(data) {
                BASE(data);
                this.dom.find('iframe').$
                    .load(function () {
                        this.dom.find('iframe').parent()
                            .removeClass('partial-update');
                    }.bind(this))
            }
        ]);
});