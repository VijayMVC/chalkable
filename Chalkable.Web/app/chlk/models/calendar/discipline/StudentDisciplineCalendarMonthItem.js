REQUIRE('ria.serialize.SJX');
REQUIRE('chlk.models.calendar.BaseCalendarMonthItem');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.discipline.DisciplineTypeSummary');
REQUIRE('chlk.models.discipline.Discipline');

NAMESPACE('chlk.models.calendar.discipline', function () {
    "use strict";
    var SJX = ria.serialize.SJX;

    /** @class chlk.models.calendar.discipline.StudentDisciplineCalendarMonthItem*/
    CLASS(
        'StudentDisciplineCalendarMonthItem',  EXTENDS(chlk.models.calendar.BaseCalendarMonthItem), IMPLEMENTS(ria.serialize.IDeserializable), [

            Number, 'moreCount',

            ArrayOf(chlk.models.discipline.Discipline), 'disciplines',

            VOID, function deserialize(raw) {
                this.day = SJX.fromValue(raw.day, Number);
                this.currentMonth = SJX.fromValue(raw.iscurrentmonth, Boolean);
                this.sunday = SJX.fromValue(raw.sunday, Boolean);
                this.date = SJX.fromDeserializable(raw.date, chlk.models.common.ChlkDate);
                this.todayClassName = SJX.fromValue(raw.todayclassname, String);
                this.className = SJX.fromValue(raw.classname, String);
                this.moreCount = SJX.fromValue(raw.morecount, Number);
                this.disciplines = SJX.fromArrayOfDeserializables(raw.disciplines, chlk.models.discipline.Discipline);
            }
    ]);
});
