REQUIRE('chlk.templates.school.SchoolPeople');
REQUIRE('chlk.models.school.SchoolPeoplePart');
REQUIRE('chlk.models.id.SchoolId');

NAMESPACE('chlk.templates.school', function () {

    /** @class chlk.templates.school.SchoolPeoplePart*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/school/SchoolPeoplePart.jade')],
        [ria.templates.ModelBind(chlk.models.school.SchoolPeoplePart)],
        'SchoolPeoplePart', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.common.PaginatedList, 'users',
            [ria.templates.ModelPropertyBind],
            Number, 'selectedIndex',
            [ria.templates.ModelPropertyBind],
            Boolean, 'byLastName',
            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolId, 'schoolId'
        ])
});
