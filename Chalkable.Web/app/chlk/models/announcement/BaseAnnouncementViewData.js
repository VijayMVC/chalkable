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

                var classAvg = studentAnnouncements.getGradesAvg(count_);
                studentAnnouncements.setClassAvg && studentAnnouncements.setClassAvg(classAvg);
                return classAvg;
            }
        ]);
});
