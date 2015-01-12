REQUIRE('ria.serialize.SJX');
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

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.grading.StudentTotalPoint */
    CLASS(
        UNSAFE, 'StudentTotalPoint', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.studentId = SJX.fromValue(raw.studentid, chlk.models.id.SchoolPersonId);
                this.totalPoint = SJX.fromValue(raw.totalpoint, Number);
                this.maxTotalPoint = SJX.fromValue(raw.maxtotalpoint, Number);
            },

            READONLY, chlk.models.id.SchoolPersonId, 'studentId',
            READONLY, Number, 'totalPoint',
            READONLY, Number, 'maxTotalPoint',

            String, function displayTotalPoint(){
                var totalPoint = this.getTotalPoint();
                return this.getMaxTotalPoint() && (totalPoint || totalPoint == 0)
                    ? (totalPoint.toFixed(2).toString() + '/' + this.getMaxTotalPoint().toString()) : '';
            }
        ]);

    /** @class chlk.models.grading.ShortGradingClassSummaryGridItems*/
    CLASS(
        UNSAFE, 'ShortGradingClassSummaryGridItems', IMPLEMENTS(ria.serialize.IDeserializable), [

            VOID, function deserialize(raw) {
                this.standardId = SJX.fromValue(raw.standardId, chlk.models.id.StandardId);
                this.categoryId = SJX.fromValue(raw.categoryId, chlk.models.id.AnnouncementTypeGradingId);
                this.ableEdit = SJX.fromValue(raw.ableEdit, Boolean);
                this.avg = SJX.fromValue(raw.avg, Number);
                this.rowIndex = SJX.fromValue(raw.rowIndex, Number);
                this.autoUpdate = SJX.fromValue(raw.autoUpdate, Boolean);
                this.ableDisplayAlphaGrades = SJX.fromValue(raw.displayalphagrades, Boolean);
                this.ableDisplayStudentAverage = SJX.fromValue(raw.displaystudentaverage, Boolean);
                this.ableDisplayTotalPoints = SJX.fromValue(raw.displaytotalpoints, Boolean);
                this.roundDisplayedAverages = SJX.fromValue(raw.rounddisplayedaverages, Boolean);
                this.gradingPeriod = SJX.fromDeserializable(raw.gradingperiod, chlk.models.schoolYear.GradingPeriod);
                this.students = SJX.fromArrayOfDeserializables(raw.students, chlk.models.grading.StudentWithAvg);
                this.studentAverages = SJX.fromArrayOfDeserializables(raw.totalavarages, chlk.models.grading.StudentAverageInfo);
                this.studentTotalPoints = SJX.fromArrayOfDeserializables(raw.totalpoints, chlk.models.grading.StudentTotalPoint);
                this.gradingItems = SJX.fromArrayOfDeserializables(raw.gradingitems, chlk.models.announcement.ShortAnnouncementViewData);

                if (raw.schoolOptions) {
                    this.schoolOptions = chlk.models.school.SchoolOption.$fromRaw(raw.schoolOptions.allowscoreentryforunexcused);
                }
            },

            ArrayOf(chlk.models.grading.StudentWithAvg), 'students',
            chlk.models.school.SchoolOption, 'schoolOptions',
            ArrayOf(chlk.models.announcement.ShortAnnouncementViewData), 'gradingItems',
            chlk.models.schoolYear.GradingPeriod, 'gradingPeriod',
            chlk.models.id.StandardId, 'standardId',
            chlk.models.id.AnnouncementTypeGradingId, 'categoryId',
            Boolean, 'ableEdit',
            Number, 'avg',
            Number, 'rowIndex',
            Boolean, 'autoUpdate',
            Boolean , 'ableDisplayAlphaGrades',
            Boolean , 'ableDisplayStudentAverage',
            Boolean , 'ableDisplayTotalPoints',
            Boolean, 'roundDisplayedAverages',
            ArrayOf(chlk.models.grading.StudentAverageInfo), 'studentAverages',
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
