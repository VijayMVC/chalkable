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
            String, 'displayname',
            [ria.templates.ModelBind],
            String, 'email',
            [ria.templates.ModelBind],
            String, 'firstname',
            [ria.templates.ModelBind],
            String, 'fullname',
            [ria.templates.ModelBind],
            String, 'gender',
            [ria.templates.ModelBind],
            String, 'grade',
            [ria.templates.ModelBind],
            Number, 'id',
            [ria.templates.ModelBind],
            String, 'lastname',
            [ria.templates.ModelBind],
            String, 'localid',
            [ria.templates.ModelBind],
            String, 'roledescription',
            [ria.templates.ModelBind],
            String, 'rolename',
            [ria.templates.ModelBind],
            Number, 'schoolid'
        ])
});