REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.school.School');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.School*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.school.School)],
        'School', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            Number, 'id',
            [ria.templates.ModelPropertyBind],
            String, 'name',
            [ria.templates.ModelPropertyBind],
            String, 'localId',
            [ria.templates.ModelPropertyBind],
            String, 'ncesId',
            [ria.templates.ModelPropertyBind],
            String, 'schoolType',
            [ria.templates.ModelPropertyBind],
            String, 'schoolUrl',
            [ria.templates.ModelPropertyBind],
            String, 'timezoneId',
            [ria.templates.ModelPropertyBind],
            Boolean, 'sendEmailNotifications',
            Number, 'districtId'
        ])
});