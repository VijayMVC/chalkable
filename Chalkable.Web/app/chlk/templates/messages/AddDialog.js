REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.messages.Message');

NAMESPACE('chlk.templates.messages', function () {

    /** @class chlk.templates.messages.AddDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/messages/AddDialog.jade')],
        [ria.templates.ModelBind(chlk.models.messages.Message)],
        'AddDialog', EXTENDS(chlk.templates.JadeTemplate), [

        ])
});
