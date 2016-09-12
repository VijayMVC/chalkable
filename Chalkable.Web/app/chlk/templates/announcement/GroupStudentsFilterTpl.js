REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.announcement.GroupStudentsFilterViewData');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.GroupStudentsFilterTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/GroupStudentsFilter.jade')],
        [ria.templates.ModelBind(chlk.models.announcement.GroupStudentsFilterViewData)],
        'GroupStudentsFilterTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.GroupId, 'groupId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolYearId, 'schoolYearId',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.GradeLevelId, 'gradeLevelId',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.classes.CourseType), 'courseTypes'
        ])
});