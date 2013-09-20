REQUIRE('chlk.templates.JadeTemplate');

NAMESPACE('chlk.templates.calendar', function () {
    "use strict";
    /** @class chlk.templates.calendar.BaseCalendarTpl*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.calendar.BaseCalendar)],
        'BaseCalendarTpl', EXTENDS(chlk.templates.JadeTemplate), [

            [ria.templates.ModelPropertyBind],
            Number, 'selectedTypeId',

            [ria.templates.ModelPropertyBind],
            String, 'currentTitle',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'nextDate',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'prevDate',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'currentDate'
        ])
});