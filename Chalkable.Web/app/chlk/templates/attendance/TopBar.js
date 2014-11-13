REQUIRE('chlk.templates.classes.TopBar');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.TopBar*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/TopBar.jade')],
        [ria.templates.ModelBind(chlk.models.classes.ClassForTopBar)],
        'TopBar', EXTENDS(chlk.templates.classes.TopBar), [])
});