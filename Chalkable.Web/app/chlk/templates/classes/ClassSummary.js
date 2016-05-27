REQUIRE('chlk.templates.profile.ClassProfileTpl');
REQUIRE('chlk.models.classes.ClassProfileSummaryViewData');

NAMESPACE('chlk.templates.classes', function () {

    /** @class chlk.templates.classes.ClassSummary*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassSummary.jade')],
        [ria.templates.ModelBind(chlk.models.classes.ClassProfileSummaryViewData)],
        'ClassSummary', EXTENDS(chlk.templates.profile.ClassProfileTpl), [

            chlk.models.classes.ClassSummary, function getClassSummary(){return this.getClazz();}
        ])
});