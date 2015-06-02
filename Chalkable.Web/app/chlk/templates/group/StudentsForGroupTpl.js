REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.group.StudentsForGroupViewData');

NAMESPACE('chlk.templates.group', function () {

    /** @class chlk.templates.group.StudentsForGroupTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AnnouncementGradeLevelStudents.jade')],
        [ria.templates.ModelBind(chlk.models.group.StudentsForGroupViewData)],
        'StudentsForGroupTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.group.StudentForGroup), 'students',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.GroupId, 'groupId'
        ])
});
