REQUIRE('chlk.models.grading.StudentWithAvg');
REQUIRE('chlk.models.common.NameId');
REQUIRE('chlk.models.announcement.ShortAnnouncementViewData');
REQUIRE('chlk.models.grading.StudentAverageInfo');
REQUIRE('chlk.models.schoolYear.GradingPeriod');
REQUIRE('chlk.models.id.StandardId');
REQUIRE('chlk.models.id.AnnouncementTypeGradingId');
REQUIRE('chlk.models.school.SchoolOption');

NAMESPACE('chlk.models.grading', function () {
    "use strict";

    /** @class chlk.models.grading.StudentTotalPoint*/
    CLASS('StudentTotalPoint',[

        [ria.serialize.SerializeProperty('studentid')],
        chlk.models.id.SchoolPersonId, 'studentId',
        [ria.serialize.SerializeProperty('totalpoint')],
        Number, 'totalPoint',
        [ria.serialize.SerializeProperty('maxtotalpoint')],
        Number, 'maxTotalPoint',

        String, function displayTotalPoint(){
            var totalPoint = this.getTotalPoint();
            return this.getMaxTotalPoint() && (totalPoint || totalPoint == 0)
                ? (totalPoint.toFixed(2).toString() + '/' + this.getMaxTotalPoint().toString()) : '';
        }
    ]);

    /** @class chlk.models.grading.ShortGradingClassSummaryGridItems*/
    CLASS(
        'ShortGradingClassSummaryGridItems', [
            ArrayOf(chlk.models.grading.StudentWithAvg), 'students',

            chlk.models.school.SchoolOption, 'schoolOptions',

            [ria.serialize.SerializeProperty('gradingitems')],
            ArrayOf(chlk.models.announcement.ShortAnnouncementViewData), 'gradingItems',

            [ria.serialize.SerializeProperty('gradingperiod')],
            chlk.models.schoolYear.GradingPeriod, 'gradingPeriod',

            chlk.models.id.StandardId, 'standardId',

            chlk.models.id.AnnouncementTypeGradingId, 'categoryId',

            Boolean, 'ableEdit',

            Number, 'avg',

            Number, 'rowIndex',

            Boolean, 'autoUpdate',

            [ria.serialize.SerializeProperty('displayalphagrades')],
            Boolean , 'ableDisplayAlphaGrades',

            [ria.serialize.SerializeProperty('displaystudentaverage')],
            Boolean , 'ableDisplayStudentAverage',

            [ria.serialize.SerializeProperty('displaytotalpoints')],
            Boolean , 'ableDisplayTotalPoints',

            [ria.serialize.SerializeProperty('rounddisplayedaverages')],
            Boolean, 'roundDisplayedAverages',

            [ria.serialize.SerializeProperty('totalavarages')],
            ArrayOf(chlk.models.grading.StudentAverageInfo), 'studentAverages',

            [ria.serialize.SerializeProperty('totalpoints')],
            ArrayOf(chlk.models.grading.StudentTotalPoint), 'studentTotalPoints',

            function getTooltipText(){
                return (this.getAvg() != null ? Msg.Avg + " " + this.getAvg() : 'No grades yet');
            },

            [[chlk.models.grading.StudentAverageInfo]],
            String, function displayAvgName(studentAverage){
                return studentAverage && studentAverage.isGradingPeriodAverage()
                    ? Msg.Avg : studentAverage.getAverageName();
            },

            String, function displayGrade(grade){
                return grade ? grade.toFixed(2) : '';
            },

            String, function displayAverages(average){
                var numbersCount = this.isRoundDisplayedAverages() ? 0 : 2;
                return average ? parseFloat(average).toFixed(numbersCount) : '';
            },

            [[chlk.models.grading.ShortStudentAverageInfo, Boolean, Boolean]],
            String, function displayAvgGradeValue(averageInfo, isAbleDisplayAlphaGrades_, original_){
                var alphaGrade = original_ ? averageInfo.getCalculatedAlphaGrade() : averageInfo.getAlphaGrade();
                var res = this.displayGrade(original_ ? averageInfo.getCalculatedAvg() : averageInfo.getNumericAvg());
                if(res && isAbleDisplayAlphaGrades_ && alphaGrade && alphaGrade.trim() != ''){
                    res += '(' + alphaGrade + ')';
                }
                return res;
            }
        ]);
});
