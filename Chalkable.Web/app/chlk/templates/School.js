REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.School');

NAMESPACE('chlk.templates', function () {

    /** @class chlk.templates.School*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/School.jade')],
        [ria.templates.ModelBind(chlk.models.School)],
        'School', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelBind],
            Number, 'id',
            [ria.templates.ModelBind],
            String, 'name',
            [ria.templates.ModelBind],
            String, 'localid',
            [ria.templates.ModelBind],
            String, 'ncesid',
            [ria.templates.ModelBind],
            String, 'schooltype',
            [ria.templates.ModelBind],
            String, 'schoolurl',
            [ria.templates.ModelBind],
            String, 'timezoneid',
            [ria.templates.ModelBind],
            Boolean, 'sendemailnotifications',
            Boolean, 'renderLink'
        ])
});