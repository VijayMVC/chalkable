REQUIRE('chlk.models.announcement.AnnouncementForm');
REQUIRE('chlk.models.common.NameId');

NAMESPACE('chlk.models.announcement', function () {
    "use strict";

    /** @class chlk.models.announcement.LessonPlanForm*/
    CLASS(
        'LessonPlanForm', EXTENDS(chlk.models.announcement.AnnouncementForm), []);
});
