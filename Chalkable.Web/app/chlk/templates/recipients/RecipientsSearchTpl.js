REQUIRE('chlk.templates.search.SiteSearch');

NAMESPACE('chlk.templates.recipients', function () {

    /** @class chlk.templates.recipients.RecipientsSearchTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/recipients/RecipientsSearchItem.jade')],
        [ria.templates.ModelBind(chlk.models.search.SearchItem)],
        'RecipientsSearchTpl', EXTENDS(chlk.templates.search.SiteSearch), [])
});