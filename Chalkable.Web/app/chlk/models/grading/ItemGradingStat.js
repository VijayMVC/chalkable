REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.grading.GraphPoint');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.ItemGradingStat*/
    CLASS(
        'ItemGradingStat', [
            [ria.serialize.SerializeProperty('announcementid')],
            chlk.models.id.AnnouncementId, 'announcementId',
            [ria.serialize.SerializeProperty('gradingstyle')],
            Number, 'gradingStyle',
            [ria.serialize.SerializeProperty('graphpoints')],
            ArrayOf(chlk.models.grading.GraphPoint), 'graphPoints'
        ]);
});
