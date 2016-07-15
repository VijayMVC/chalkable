REQUIRE('chlk.templates.ChlkTemplate');

REQUIRE('chlk.models.school.School');
REQUIRE('chlk.models.id.SchoolId');
REQUIRE('chlk.models.id.DistrictId');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.School*/
    CLASS(
        [ria.templates.ModelBind(chlk.models.school.School)],
        'School', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolId, 'id',
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
            [ria.templates.ModelPropertyBind],
            chlk.models.id.DistrictId, 'districtId'
        ])
});