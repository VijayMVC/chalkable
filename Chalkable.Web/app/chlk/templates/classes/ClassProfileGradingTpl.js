REQUIRE('chlk.templates.profile.ClassProfileTpl');
REQUIRE('chlk.models.classes.BaseGradingClassProfileViewData');

NAMESPACE('chlk.templates.classes', function () {

    /** @class chlk.templates.classes.ClassProfileGradingTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassProfileGrading.jade')],
        [ria.templates.ModelBind(chlk.models.classes.BaseGradingClassProfileViewData)],
        'ClassProfileGradingTpl', EXTENDS(chlk.templates.profile.ClassProfileTpl), [

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.GradingPageTypeEnum, 'gradingPageType',

            function getTemplateClass(){
                switch(this.getGradingPageType()){
                    case chlk.models.classes.GradingPageTypeEnum.ITEMS_BOXES: return chlk.templates.grading.GradingClassSummaryTpl;
                    case chlk.models.classes.GradingPageTypeEnum.ITEMS_GRID: return chlk.templates.grading.GradingClassSummaryGridTpl;
                    case chlk.models.classes.GradingPageTypeEnum.STANDARDS_BOXES: return chlk.templates.grading.GradingClassStandardsTpl;
                    case chlk.models.classes.GradingPageTypeEnum.STANDARDS_GRID: return chlk.templates.grading.GradingClassStandardsGridTpl;
                    case chlk.models.classes.GradingPageTypeEnum.FINAL_GRADES: return chlk.templates.grading.FinalGradesTpl;
                }
            }
        ])
});