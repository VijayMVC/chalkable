REQUIRE('chlk.models.classes.Class');
REQUIRE('chlk.models.common.HoverBox');
//REQUIRE('chlk.models.common.CommonHoverBox');
REQUIRE('chlk.models.classes.Room');
REQUIRE('chlk.models.announcement.AnnouncementsByDate');

REQUIRE('chlk.models.common.DisciplineHoverBoxItem');
REQUIRE('chlk.models.common.HoverBoxItem');

NAMESPACE('chlk.models.classes', function () {
    "use strict";

    /** @class chlk.models.classes.ClassSummary*/
    CLASS(
        'ClassSummary', EXTENDS(chlk.models.classes.Class), [
            chlk.models.classes.Room, 'room',

            ArrayOf(chlk.models.people.User), 'students',

            [ria.serialize.SerializeProperty('classsize')],
            Number, 'classSize',

            [ria.serialize.SerializeProperty('classattendancebox')],
            chlk.models.common.HoverBox.OF(chlk.models.common.HoverBoxItem), 'classAttendanceBox',

            [ria.serialize.SerializeProperty('classdisciplinebox')],
            chlk.models.common.HoverBox.OF(chlk.models.common.DisciplineHoverBoxItem), 'classDisciplineBox',

            [ria.serialize.SerializeProperty('classaveragebox')],
            chlk.models.common.HoverBox.OF(chlk.models.common.HoverBoxItem), 'classAverageBox',

            [ria.serialize.SerializeProperty('announcementsbydate')],
            ArrayOf(chlk.models.announcement.AnnouncementsByDate), 'announcementsByDate'

        ]);
});
