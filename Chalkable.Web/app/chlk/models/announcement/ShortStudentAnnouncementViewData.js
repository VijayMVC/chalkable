
REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.id.StudentAnnouncementId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.ShortStudentAnnouncementViewData*/
    CLASS(
        'ShortStudentAnnouncementViewData', [
            [ria.serialize.SerializeProperty('announcementid')],
            chlk.models.id.AnnouncementId, 'announcementId',

            String, 'comment',

            Boolean, 'dropped',

            ArrayOf(String), 'alerts',

            [ria.serialize.SerializeProperty('gradevalue')],
            Number, 'gradeValue',

            chlk.models.id.StudentAnnouncementId, 'id',

            Number, 'state',

            [ria.serialize.SerializeProperty('islate')],
            Boolean, 'late',

            [ria.serialize.SerializeProperty('isabsent')],
            Boolean, 'absent',

            [ria.serialize.SerializeProperty('isincomplete')],
            Boolean, 'incomplete',

            [ria.serialize.SerializeProperty('isexempt')],
            Boolean, 'exempt',

            [ria.serialize.SerializeProperty('ispassed')],
            Boolean, 'passed',

            [ria.serialize.SerializeProperty('iscomplete')],
            Boolean, 'complete'
        ]);
});
