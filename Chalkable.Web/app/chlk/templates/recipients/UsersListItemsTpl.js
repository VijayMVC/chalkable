REQUIRE('chlk.templates.recipients.UsersListTpl');

NAMESPACE('chlk.templates.recipients', function () {

    /** @class chlk.templates.recipients.UsersListItemsTpl*/
    //TODO DELETE
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/recipients/UsersListItems.jade')],
        [ria.templates.ModelBind(chlk.models.recipients.UsersListViewData)],
        'UsersListItemsTpl', EXTENDS(chlk.templates.recipients.UsersListTpl), [])
});