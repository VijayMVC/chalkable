REQUIRE('chlk.models.id.StandardId');
REQUIRE('chlk.models.id.AnnouncementId');

NAMESPACE('chlk.models.standard', function () {
    "use strict";
    /** @class chlk.models.standard.Standard*/
    CLASS(
        'Standard', [
             String, 'name',

             String, 'description',

             chlk.models.id.AnnouncementId, 'announcementId',

             [ria.serialize.SerializeProperty('standardid')],
             chlk.models.id.StandardId, 'standardId'
        ]);
});
