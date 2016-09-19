REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.templates.PaginatedList');

NAMESPACE('chlk.templates.controls', function () {

    /** @class chlk.templates.controls.PersonItemsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/controls/group-people-selector/person-items.jade')],
        [ria.templates.ModelBind(chlk.models.common.PaginatedList)],
        'PersonItemsTpl', EXTENDS(chlk.templates.PaginatedList), [])
});