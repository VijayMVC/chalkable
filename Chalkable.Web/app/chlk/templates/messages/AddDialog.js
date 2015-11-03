REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.messages.Message');
REQUIRE('chlk.templates.messages.RecipientAutoComplete');
REQUIRE('chlk.models.common.ChlkDate');


NAMESPACE('chlk.templates.messages', function () {

    /** @class chlk.templates.messages.AddDialog*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/messages/AddDialog.jade')],
        [ria.templates.ModelBind(chlk.models.messages.Message)],
        'AddDialog', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            chlk.models.id.MessageId, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'subject',

            [ria.templates.ModelPropertyBind],
            String, 'body',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.User, 'sender',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.User, 'recipientPerson',

            [ria.templates.ModelPropertyBind],
            chlk.models.classes.Class, 'recipientClass',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'sent',

            [ria.templates.ModelPropertyBind],
            Boolean, 'inbox',

            [ria.templates.ModelPropertyBind],
            Boolean, 'disabledMessaging',

            function getRecipientText(){
                return this.getRecipientPerson() ? this.getRecipientPerson().getFirstName()+ ' ' + this.getRecipientPerson().getLastName() :
                    (this.getRecipientClass() ? this.getRecipientClass().getFullClassName() : '');
            },

            function getRecipientValue(){
                var personId = this.getRecipientPerson() ? this.getRecipientPerson().getId().valueOf() : null;
                var classId = this.getRecipientClass() ? this.getRecipientClass().getId().valueOf() : null;
                if(!personId && !classId)
                    return '';
                return JSON.stringify([personId, classId]);
            }
        ])
});
