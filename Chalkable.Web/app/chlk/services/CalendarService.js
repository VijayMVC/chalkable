REQUIRE('chlk.services.BaseService');
REQUIRE('ria.async.Future');

REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.calendar.announcement.Month');

REQUIRE('chlk.models.id.ClassId');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.CalendarService */
    CLASS(
        'CalendarService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.ClassId, chlk.models.common.ChlkDate]],
            ria.async.Future, function listForMonth(classId_, date_) {
                return this.get('chalkable2/app/data/calendarMonth.json', ArrayOf(chlk.models.calendar.announcement.Month), {
                    //classId: classId_.valueOf(),
                    //date: date_.getDate()
                });
            }
        ])
});