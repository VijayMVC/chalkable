REQUIRE('chlk.models.common.NameId');
REQUIRE('chlk.models.announcement.ShortAnnouncementViewData');

NAMESPACE('chlk.models.grading', function () {
    "use strict";
    /** @class chlk.models.grading.GradingClassSummaryItem*/
    CLASS(
        'GradingClassSummaryItem', [
            [ria.serialize.SerializeProperty('type')],
            chlk.models.common.NameId, 'itemDescription',

            ArrayOf(chlk.models.announcement.ShortAnnouncementViewData), 'announcements',

            Number, 'percent',

            Number, 'avg',

            Number, 'index',

            chlk.models.id.ClassId, 'classId'
        ]);
});
