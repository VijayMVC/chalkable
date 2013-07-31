REQUIRE('chlk.templates.JadeTemplate');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.Schools*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/SchoolSchedule.jade')],
        'SchoolSchedule', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.schoolYear.Year, 'year',
            [ria.templates.ModelPropertyBind],
            Number, 'sections'
        ])
});