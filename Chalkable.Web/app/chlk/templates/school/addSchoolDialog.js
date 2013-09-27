REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.school.School');
REQUIRE('chlk.models.id.SchoolId');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.AddSchoolDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/add-school-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.school.School)],
        'AddSchoolDialog', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolId, 'id',
            [ria.templates.ModelPropertyBind],
            String, 'name',
            [ria.templates.ModelPropertyBind],
            Number, 'localId',
            [ria.templates.ModelPropertyBind],
            Number, 'ncesId',
            [ria.templates.ModelPropertyBind],
            String, 'schoolType',
            [ria.templates.ModelPropertyBind],
            String, 'schoolUrl',
            [ria.templates.ModelPropertyBind],
            String, 'timezoneId',
            [ria.templates.ModelPropertyBind],
            Boolean, 'sendEmailNotifications',
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.school.Timezone), 'timezones'
        ])
});