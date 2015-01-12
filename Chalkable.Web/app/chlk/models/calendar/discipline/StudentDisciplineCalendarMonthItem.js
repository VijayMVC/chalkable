REQUIRE('chlk.models.calendar.BaseCalendarMonthItem');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.discipline.DisciplineTypeSummary');
REQUIRE('chlk.models.discipline.Discipline');

NAMESPACE('chlk.models.calendar.discipline', function () {
    "use strict";

    /** @class chlk.models.calendar.discipline.StudentDisciplineCalendarMonthItem*/
    CLASS(
        'StudentDisciplineCalendarMonthItem',  EXTENDS(chlk.models.calendar.BaseCalendarMonthItem), [

            [ria.serialize.SerializeProperty('morecount')],
            Number, 'moreCount',

            ArrayOf(chlk.models.discipline.Discipline), 'disciplines',

            [ria.serialize.SerializeProperty('disciplinetypes')],
            ArrayOf(chlk.models.discipline.DisciplineTypeSummary),'disciplineTypes'
    ]);
});
