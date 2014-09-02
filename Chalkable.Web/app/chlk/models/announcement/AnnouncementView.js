REQUIRE('chlk.models.announcement.Announcement');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.AnnouncementView*/
    CLASS(
        'AnnouncementView', EXTENDS(chlk.models.announcement.Announcement), [

            ArrayOf(chlk.models.grading.AlphaGrade), 'alphaGrades',

            ArrayOf(chlk.models.grading.AlternateScore), 'alternateScores'

        ]);
});
