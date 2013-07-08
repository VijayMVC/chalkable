REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.school.School');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.AddSchoolDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/add-school-dialog.jade')],
        [ria.templates.ModelBind(chlk.models.school.School)],
        'AddSchoolDialog', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelBind],
            Number, 'id',
            [ria.templates.ModelBind],
            String, 'name',
            [ria.templates.ModelBind],
            Number, 'localId',
            [ria.templates.ModelBind],
            Number, 'ncesId',
            [ria.templates.ModelBind],
            String, 'schoolType',
            [ria.templates.ModelBind],
            String, 'schoolUrl',
            [ria.templates.ModelBind],
            String, 'timezoneId',
            [ria.templates.ModelBind],
            Boolean, 'sendEmailNotifications',
            [ria.templates.ModelBind],
            ArrayOf(chlk.models.school.Timezone), 'timezones'
        ])
});