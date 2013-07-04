REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.school.School');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.School*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.school.School)],
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
            Boolean, 'sendEmailNotifications'
        ])
});