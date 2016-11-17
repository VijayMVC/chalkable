REQUIRE('chlk.templates.profile.ClassProfileTpl');

NAMESPACE('chlk.templates.classes', function () {
    "use strict";
    /** @class chlk.templates.classes.ClassProfileAttendanceTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/class/ClassProfileAttendance.jade')],
        [ria.templates.ModelBind(chlk.models.classes.BaseClassProfileViewData)],
        'ClassProfileAttendanceTpl', EXTENDS(chlk.templates.profile.ClassProfileTpl), [

        ]);
});