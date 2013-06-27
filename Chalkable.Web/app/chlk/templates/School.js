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
            String, 'localId',
            [ria.templates.ModelBind],
            String, 'ncesId',
            [ria.templates.ModelBind],
            String, 'schoolType',
            [ria.templates.ModelBind],
            String, 'schoolUrl',
            [ria.templates.ModelBind],
            String, 'timezoneId',
            [ria.templates.ModelBind],
            Boolean, 'sendEmailNotifications',
            [ria.templates.ModelBind],
            String, 'contactTitle',
            [ria.templates.ModelBind],
            String, 'contactName'

        ])
});