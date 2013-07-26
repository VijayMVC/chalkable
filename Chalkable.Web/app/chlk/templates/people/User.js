REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.templates', function () {

    /** @class chlk.templates.School*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/SchoolPeople.jade')],
        [ria.templates.ModelBind(chlk.models.school.School)],
        'School', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelBind],
            Boolean, 'active',
            [ria.templates.ModelBind],
            String, 'displayName',
            [ria.templates.ModelBind],
            String, 'email',
            [ria.templates.ModelBind],
            String, 'firstName',
            [ria.templates.ModelBind],
            String, 'fullName',
            [ria.templates.ModelBind],
            String, 'gender',
            [ria.templates.ModelBind],
            String, 'grade',
            [ria.templates.ModelBind],
            Number, 'id',
            [ria.templates.ModelBind],
            String, 'lastName',
            [ria.templates.ModelBind],
            String, 'localId',
            [ria.templates.ModelBind],
            String, 'roleDescription',
            [ria.templates.ModelBind],
            String, 'roleName',
            [ria.templates.ModelBind],
            Number, 'schoolId'
        ])
});