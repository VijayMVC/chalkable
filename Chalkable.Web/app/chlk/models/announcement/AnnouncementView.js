REQUIRE('chlk.models.announcement.FeedAnnouncementViewData');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementView*/
    CLASS(
        'AnnouncementView', EXTENDS(chlk.models.announcement.FeedAnnouncementViewData), [

            ArrayOf(chlk.models.grading.AlphaGrade), 'alphaGrades',

            Boolean, 'hasAccessToLE',

            ArrayOf(chlk.models.grading.AlternateScore), 'alternateScores',

            chlk.models.common.PaginatedList, 'students'
        ]);
});
