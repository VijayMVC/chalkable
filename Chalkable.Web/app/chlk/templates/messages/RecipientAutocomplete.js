REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.templates.messages', function () {

    /** @class chlk.templates.messages.RecipientAutoComplete*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/messages/RecipientAutoComplete.jade')],
        [ria.templates.ModelBind(chlk.models.people.User)],
        'RecipientAutoComplete', EXTENDS(chlk.templates.JadeTemplate), [

            [ria.templates.ModelPropertyBind],
            String, 'displayName',

            [ria.templates.ModelPropertyBind],
            String, 'email',

            [ria.templates.ModelPropertyBind],
            String, 'firstName',

            [ria.templates.ModelPropertyBind],
            String, 'fullName',

            [ria.templates.ModelPropertyBind],
            String, 'gender',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'lastName',

            [ria.templates.ModelPropertyBind],
            String, 'salutation'
        ])
});

