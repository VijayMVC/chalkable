REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.announcement.ShortStudentAnnouncementsViewData');
REQUIRE('chlk.models.announcement.BaseAnnouncementViewData');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.announcement.ShortAnnouncementViewData*/
    CLASS(
        UNSAFE, 'ShortAnnouncementViewData', EXTENDS(chlk.models.announcement.BaseAnnouncementViewData), [
            chlk.models.announcement.ShortStudentAnnouncementsViewData, 'studentAnnouncements',

            Boolean, 'ableDropStudentScore',
            Boolean, 'ableExemptStudentScore',
            Number, 'order',
            chlk.models.common.ChlkDate, 'expiresDate',
            Number, 'avg',
            Boolean, 'dropped',
            Number, 'maxScore',
            Boolean, 'gradable',
            Boolean, 'ableToGrade',

            OVERRIDE, VOID, function deserialize(raw) {
                BASE(raw);
                this.ableDropStudentScore = SJX.fromValue(raw.candropstudentscore, Boolean);
                this.ableExemptStudentScore = SJX.fromValue(raw.maybeexempt, Boolean);
                this.order = SJX.fromValue(raw.order, Number);
                this.expiresDate = SJX.fromDeserializable(raw.expiresdate, chlk.models.common.ChlkDate);
                this.avg = SJX.fromValue(raw.avg, Number);
                this.dropped = SJX.fromValue(raw.dropped, Boolean);
                this.maxScore = SJX.fromValue(raw.maxscore, Number);
                this.gradable = SJX.fromValue(raw.gradable, Boolean);
                this.ableToGrade = SJX.fromValue(raw.cangrade, Boolean);
                this.studentAnnouncements = ria.serialize.SJX.fromDeserializable(raw.studentannouncements, chlk.models.announcement.ShortStudentAnnouncementsViewData);
            },

            String, function calculateGradesAvg(count_) {
                var studentAnnouncements = this.getStudentAnnouncements();
                if (!studentAnnouncements)
                    return null;

                var classAvg = studentAnnouncements.getGradesAvg(count_);
                studentAnnouncements.setClassAvg(classAvg);
                return classAvg;
            }
        ]);
});
