REQUIRE('chlk.models.standard.StandardGradings');
REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.grading.GradingClassSummaryGridItems');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.ShortGradingClassStandardsGridItemsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/ShortGradingClassStandardsGridItems.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradingClassSummaryGridItems.OF(chlk.models.standard.StandardGradings))],
        'ShortGradingClassStandardsGridItemsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.common.NameId, 'gradingPeriod',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.StudentWithAvg), 'students',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.standard.StandardGradings), 'gradingItems',

            [ria.templates.ModelPropertyBind],
            Number, 'avg',

            [ria.templates.ModelPropertyBind],
            chlk.models.school.SchoolOption, 'schoolOptions',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableEdit',

            [ria.templates.ModelPropertyBind],
            Boolean, 'gradable',

            chlk.models.id.ClassId, 'classId',

            ArrayOf(chlk.models.standard.StandardGradings), function getGradingItemsOrdered() {
                Assert(Array.isArray(this.gradingItems));
                return (this.gradingItems || []).sort(function (_1, _2) {
                    return _1.getTitle() < _2.getTitle() ? -1 : _1.getTitle() > _2.getTitle() ? 1 : 0;
                });
            },

            String, function displayGrade(grade){
                if(isNaN(parseFloat(grade)))
                    return grade;
                return grade || grade == 0 ? parseFloat(grade).toFixed(2) : '';
            }
        ]);
});
