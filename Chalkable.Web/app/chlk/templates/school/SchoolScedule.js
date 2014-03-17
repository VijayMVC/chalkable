REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.Schools*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/SchoolSchedule.jade')],
        'SchoolSchedule', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.schoolYear.Year, 'year',
            [ria.templates.ModelPropertyBind],
            Number, 'sections'
        ])
});