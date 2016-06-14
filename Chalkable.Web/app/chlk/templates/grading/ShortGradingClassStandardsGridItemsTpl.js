REQUIRE('chlk.models.standard.StandardGradings');
REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.grading.GradingClassSummaryGridItems');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.ShortGradingClassStandardsGridItemsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/ShortGradingClassStandardsGridItems.jade')],
        [ria.templates.ModelBind(chlk.models.grading.GradingClassSummaryGridItems.OF(Object))],
        'ShortGradingClassStandardsGridItemsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.common.NameId, 'gradingPeriod',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.StudentWithAvg), 'students',

            [ria.templates.ModelPropertyBind],
            ArrayOf(Object), 'gradingItems',

            [ria.templates.ModelPropertyBind],
            Number, 'avg',

            [ria.templates.ModelPropertyBind],
            chlk.models.school.SchoolOption, 'schoolOptions',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableEdit',

            [ria.templates.ModelPropertyBind],
            Boolean, 'gradable',

            chlk.models.id.ClassId, 'classId',

            Array, function getGradingItemsOrdered() {
                Assert(Array.isArray(this.gradingItems));
                return (this.gradingItems || []).sort(function (_1, _2) {
                    return _1.standard.name < _2.standard.name ? -1 : _1.standard.name > _2.standard.name ? 1 : 0;
                });
            },

            String, function displayGrade(grade){
                if(isNaN(parseFloat(grade)))
                    return grade;
                return grade || grade == 0 ? parseFloat(grade).toFixed(2) : '';
            },

            function getTbWidth(){
                return ria.dom.Dom('#content').width() - 412;
            },

            String, function getTooltip(gradingItem){
                var standard = gradingItem.standard;
                return standard.description ? standard.name + " | " + standard.description
                    : standard.name;
            },
        ]);
});
