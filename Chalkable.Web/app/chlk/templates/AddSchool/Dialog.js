REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.School');

NAMESPACE('chlk.templates.AddSchool', function () {

    /** @class chlk.templates.AddSchool.Dialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/add-school/dialog.jade')],
        [ria.templates.ModelBind(chlk.models.School)],
        'Dialog', EXTENDS(chlk.templates.JadeTemplate), [
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
            Boolean, 'sendEmailNotifications'
        ])
});