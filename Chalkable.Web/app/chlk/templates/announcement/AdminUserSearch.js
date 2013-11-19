REQUIRE('chlk.templates.people.User');

NAMESPACE('chlk.templates.announcement', function () {

    /** @class chlk.templates.announcement.AdminUserSearch*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/announcement/AdminUserSearch.jade')],
        [ria.templates.ModelBind(chlk.models.people.User)],
        'AdminUserSearch', EXTENDS(chlk.templates.people.User), [])
});