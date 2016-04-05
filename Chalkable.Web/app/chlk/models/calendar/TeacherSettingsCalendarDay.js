REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.period.ClassPeriod');

NAMESPACE('chlk.models.calendar', function () {
    "use strict";

    /** @class chlk.models.calendar.TeacherSettingsCalendarDay*/
    CLASS(
        'TeacherSettingsCalendarDay', [
            chlk.models.common.ChlkDate, 'date',

            Number, 'day',

            [ria.serialize.SerializeProperty('classperiods')],
            ArrayOf(chlk.models.period.ClassPeriod), 'classPeriods'
        ]);
});
