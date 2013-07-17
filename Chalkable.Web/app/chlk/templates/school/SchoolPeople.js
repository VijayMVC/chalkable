REQUIRE('chlk.models.school.SchoolPeopleSummary');
REQUIRE('chlk.models.school.SchoolPeople');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.SchoolPeople*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/SchoolPeople.jade')],
        [ria.templates.ModelBind(chlk.models.school.SchoolPeople)],
        'SchoolPeople', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'users',
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.NameId), 'roles',
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.common.NameId), 'gradeLevels',
            [ria.templates.ModelPropertyBind],
            chlk.models.school.SchoolPeopleSummary, 'schoolInfo',
            [ria.templates.ModelPropertyBind],
            Number, 'selectedIndex',
            [ria.templates.ModelPropertyBind],
            Boolean, 'byLastName'
        ])
});
