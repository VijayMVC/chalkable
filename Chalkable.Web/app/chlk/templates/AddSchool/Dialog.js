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