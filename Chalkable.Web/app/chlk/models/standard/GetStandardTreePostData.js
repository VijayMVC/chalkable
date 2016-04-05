REQUIRE('chlk.models.standard.Standard');
REQUIRE('chlk.models.id.ClassId');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.standard', function () {
    "use strict";
    var SJX = ria.serialize.SJX;

    /** @class chlk.models.standard.GetStandardTreePostData*/
    CLASS(
        'GetStandardTreePostData', [

            [ria.serialize.SerializeProperty('classid')],
            chlk.models.id.ClassId, 'classId',
            [ria.serialize.SerializeProperty('standardid')],
            chlk.models.id.StandardId, 'standardId',
            [ria.serialize.SerializeProperty('announcementid')],
            chlk.models.id.AnnouncementId, 'announcementId',

            String, 'filter'

        ]);
});
