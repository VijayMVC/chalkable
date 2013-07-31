REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.developer.DeveloperInfo');

NAMESPACE('chlk.templates.profile', function () {

    /** @class chlk.templates.profile.DeveloperProfile*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/profile/DeveloperProfile.jade')],
        [ria.templates.ModelBind(chlk.models.developer.DeveloperInfo)],
        'DeveloperProfile', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.DeveloperId, 'id',
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
            String, 'webSite'
        ])
});