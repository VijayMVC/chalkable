REQUIRE('chlk.models.grading.ShortGradingClassSummaryGridItems');
REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.schoolYear.GradingPeriod');

NAMESPACE('chlk.templates.grading', function () {
    "use strict";
    /** @class chlk.templates.grading.ShortGradingClassSummaryGridItemsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/grading/TeacherClassGradingGridSummaryItem.jade')],
        [ria.templates.ModelBind(chlk.models.grading.ShortGradingClassSummaryGridItems)],
        'ShortGradingClassSummaryGridItemsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.StudentWithAvg), 'students',

            [ria.templates.ModelPropertyBind],
            chlk.models.school.SchoolOption, 'schoolOptions',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.announcement.ShortAnnouncementViewData), 'gradingItems',

            [ria.templates.ModelPropertyBind],
            chlk.models.schoolYear.GradingPeriod, 'gradingPeriod',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.StandardId, 'standardId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.AnnouncementTypeGradingId, 'categoryId',

            [ria.templates.ModelPropertyBind],
            Number, 'avg',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableEdit',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableEditAvg',

            [ria.templates.ModelPropertyBind],
            Boolean, 'autoUpdate',

            [ria.templates.ModelPropertyBind],
            Number, 'rowIndex',

            [ria.templates.ModelPropertyBind],
            Boolean , 'ableDisplayAlphaGrades',

            [ria.templates.ModelPropertyBind],
            Boolean , 'ableDisplayStudentAverage',

            [ria.templates.ModelPropertyBind],
            Boolean , 'ableDisplayTotalPoints',

            [ria.templates.ModelPropertyBind],
            Boolean, 'roundDisplayedAverages',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.StudentAverageInfo), 'studentAverages',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.StudentTotalPoint), 'studentTotalPoints',

            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            Boolean, 'inProfile',

            function getTbWidth(){
                var res = ria.dom.Dom('#content').width() - 412;
                if(this.isAbleDisplayStudentAverage() || this.isAbleDisplayAlphaGrades()){
                    var avgs = ria.dom.Dom('.avgs-container');
                    res = res - 128 * this.getStudentAverages().length;
                }

                return res;
            },

            function getStudentIds(){
                var res = [];
                this.getStudents().forEach(function(item){
                    res.push(item.getStudentInfo().getId().valueOf());
                });
                return res.join(',');
            }
        ]);
});
