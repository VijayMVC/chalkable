REQUIRE('chlk.models.school.SchoolSisInfo');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.SchoolSisInfo*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/SchoolSis.jade')],
        [ria.templates.ModelBind(chlk.models.school.SchoolSisInfo)],
        'SchoolSisInfo', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            Number, 'attendanceSyncFreq',
            [ria.templates.ModelPropertyBind],
            Number, 'disciplineSyncFreq',
            [ria.templates.ModelPropertyBind],
            Number, 'personSyncFreq',
            [ria.templates.ModelPropertyBind],
            Number, 'scheduleSyncFreq',
            [ria.templates.ModelPropertyBind],
            Number, 'id',
            [ria.templates.ModelPropertyBind],
            String, 'sisUrl',
            [ria.templates.ModelPropertyBind],
            String, 'sisUserName',
            [ria.templates.ModelPropertyBind],
            String, 'sisPassword',
            [ria.templates.ModelPropertyBind],
            String, 'sisName'
        ])
});
