REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementView*/
    CLASS(
        'AnnouncementView', EXTENDS(chlk.models.announcement.FeedAnnouncementViewData), [
            ArrayOf(chlk.models.grading.AlphaGrade), 'alphaGrades',

            Boolean, 'hasAccessToLE',

            ArrayOf(chlk.models.grading.AlternateScore), 'alternateScores'

        ]);
});
