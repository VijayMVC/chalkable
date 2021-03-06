REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.teacher.StudentsList');

NAMESPACE('chlk.templates.teacher', function () {
    "use strict";
    /** @class chlk.templates.teacher.StudentsList*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/teacher/StudentsList.jade')],
        [ria.templates.ModelBind(chlk.models.teacher.StudentsList)],
        'StudentsList', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.people.UsersList, 'usersList', //todo: rename

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.ClassesForTopBar, 'topData', //todo: rename

            [ria.templates.ModelPropertyBind],
            Boolean, 'hasAccessToLE',

            [ria.templates.ModelPropertyBind],
            Boolean, 'my',

            [ria.templates.ModelPropertyBind],
            Boolean, 'hasAccessToAll'
        ]);
});
