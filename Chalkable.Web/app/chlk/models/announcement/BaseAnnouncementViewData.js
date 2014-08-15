REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.BaseAnnouncementViewData*/
    CLASS(
        'BaseAnnouncementViewData', [
            [ria.serialize.SerializeProperty('announcementtypename')],
            String, 'announcementTypeName',

            [ria.serialize.SerializeProperty('candropstudentscore')],
            Boolean, 'ableDropStudentScore',

            [ria.serialize.SerializeProperty('maybeexempt')],
            Boolean, 'ableExemptStudentScore',

            Number, 'order',

            String, 'title',

            [ria.serialize.SerializeProperty('expiresdate')],
            chlk.models.common.ChlkDate, 'expiresDate',

            Number, 'avg',

            chlk.models.id.AnnouncementId, 'id',

            Boolean, 'dropped',

            [ria.serialize.SerializeProperty('maxscore')],
            Number, 'maxScore',

            [ria.serialize.SerializeProperty('isowner')],
            Boolean, 'annOwner',

            Boolean, 'gradable',

            [ria.serialize.SerializeProperty('cangrade')],
            Boolean, 'ableToGrade',

            function calculateGradesAvg(count_){
                var studentAnnouncements = this.getStudentAnnouncements();
                if (!studentAnnouncements)
                    return null;

                var gradedStudentCount = 0, sum = 0, numericGrade, gradeValue;
                var items = studentAnnouncements.getItems() || [], classAvg = null;
                items.forEach(function(item){
                    numericGrade = item.getNumericGradeValue();
                    gradeValue = item.getGradeValue();
                    if(!item.isDropped()
                        && !item.isIncomplete()
                        && (gradeValue && gradeValue.toLowerCase() != 'ps'
                            && gradeValue.toLowerCase() != 'wd'
                            && gradeValue.toLowerCase() != 'nc')
                        && (numericGrade || numericGrade == 0 || gradeValue == 0 || gradeValue)){
                            gradedStudentCount++;
                            sum += (numericGrade || 0);
                    }
                });
                studentAnnouncements.setGradedStudentCount && studentAnnouncements.setGradedStudentCount(gradedStudentCount);
                if(gradedStudentCount){
                    if(count_)
                        classAvg = (sum / gradedStudentCount).toFixed(count_);
                    else
                        classAvg = Math.floor(sum / gradedStudentCount + 0.5);
                }
                studentAnnouncements.setClassAvg && studentAnnouncements.setClassAvg(classAvg);
                return classAvg;
            }
        ]);
});
