REQUIRE('chlk.models.school.SchoolSisInfo');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.SchoolSisInfo*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/SchoolSis.jade')],
        [ria.templates.ModelBind(chlk.models.school.SchoolSisInfo)],
        'SchoolSisInfo', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelBind],
            Number, 'attendanceSyncFreq',
            [ria.templates.ModelBind],
            Number, 'disciplineSyncFreq',
            [ria.templates.ModelBind],
            Number, 'personSyncFreq',
            [ria.templates.ModelBind],
            Number, 'scheduleSyncFreq',
            [ria.templates.ModelBind],
            Number, 'id'
        ])
});
