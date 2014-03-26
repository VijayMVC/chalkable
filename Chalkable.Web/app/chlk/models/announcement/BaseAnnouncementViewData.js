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
            Boolean, 'annOwner'
        ]);
});
