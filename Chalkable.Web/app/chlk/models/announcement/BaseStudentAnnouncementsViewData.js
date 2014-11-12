NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.BaseStudentAnnouncementsViewData*/
    CLASS(
        'BaseStudentAnnouncementsViewData', [
            function getGradesAvg(count_){
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
                this.setGradedStudentCount && this.setGradedStudentCount(gradedStudentCount);
                if(gradedStudentCount){
                    if(count_)
                        classAvg = (sum / gradedStudentCount).toFixed(count_);
                    else
                        classAvg = Math.floor(sum / gradedStudentCount + 0.5);
                }
                return classAvg;
            }
        ]);
});
