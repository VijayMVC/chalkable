REQUIRE('chlk.models.id.AnnouncementId');
REQUIRE('chlk.models.grading.GraphPoint');
REQUIRE('chlk.models.announcement.ShortStudentAnnouncementsViewData');
REQUIRE('chlk.models.announcement.ShortStudentAnnouncementViewData');

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
            ArrayOf(chlk.models.grading.GraphPoint), 'graphPoints',

            [ria.serialize.SerializeProperty('studentannouncements')],
            ArrayOf(chlk.models.announcement.ShortStudentAnnouncementViewData), 'items',

            READONLY, Number, 'classAvg',

            Number, function getClassAvg(){
                var model = new chlk.models.announcement.ShortStudentAnnouncementsViewData(this.items);
                return model.getGradesAvg(2);
            }
        ]);
});
