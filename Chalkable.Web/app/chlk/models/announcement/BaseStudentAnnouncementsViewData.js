REQUIRE('chlk.models.announcement.ShortStudentAnnouncementViewData');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.BaseStudentAnnouncementsViewData*/
    CLASS(
        GENERIC('T', ClassOf(chlk.models.announcement.ShortStudentAnnouncementViewData)),
        UNSAFE, 'BaseStudentAnnouncementsViewData', [

            Number, 'gradedStudentCount',
            String, 'classAvg',
            Array, 'items', // Of(T)

            [[Number]],
            String, function getGradesAvg(count_){
                var gradedStudentCount = 0, sum = 0, numericGrade, gradeValue;
                var items = this.getItems() || [], classAvg = null;
                items.forEach(function(item){
                    numericGrade = item.getNumericGradeValue();
                    gradeValue = item.getGradeValue();
                    if(!item.isDropped()
                        && !item.isIncomplete()
                        && (gradeValue && gradeValue.toLowerCase() != 'ps'
                            && gradeValue.toLowerCase() != 'wd'
                            && gradeValue.toLowerCase() != 'nc')
                        && item.isIncludeInAverage()
                        && (numericGrade || numericGrade == 0 || gradeValue == 0 || gradeValue)){
                            gradedStudentCount++;
                            sum += (numericGrade || 0);
                    }
                });
                this.setGradedStudentCount(gradedStudentCount);
                if(gradedStudentCount){
                    classAvg = (sum / gradedStudentCount).toFixed(count_ || 0);
                }
                return classAvg;
            }
        ]);
});
