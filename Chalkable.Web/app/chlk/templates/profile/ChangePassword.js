REQUIRE('chlk.templates.JadeTemplate');

NAMESPACE('chlk.templates.profile', function () {

    /** @class chlk.templates.profile.ChangePassword*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/profile/ChangePassword.jade')],
        [ria.templates.ModelBind(Class)],
        'ChangePassword', EXTENDS(chlk.templates.JadeTemplate), [
            /*[ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'id',
            [ria.templates.ModelPropertyBind],
            String, 'displayName',
            [ria.templates.ModelPropertyBind],
            String, 'email',
            [ria.templates.ModelPropertyBind],
            String, 'firstName',
            [ria.templates.ModelPropertyBind],
            String, 'lastName',
            [ria.templates.ModelPropertyBind],
            String, 'name',
            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolId, 'schoolId',
            [ria.templates.ModelPropertyBind],
            String, 'webSite'*/
        ])
});
