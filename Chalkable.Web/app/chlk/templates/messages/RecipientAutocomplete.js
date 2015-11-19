REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.messages.RecipientViewData');

NAMESPACE('chlk.templates.messages', function () {

    /** @class chlk.templates.messages.RecipientAutoComplete*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/messages/RecipientAutoComplete.jade')],
        [ria.templates.ModelBind(chlk.models.messages.RecipientViewData)],
        'RecipientAutoComplete', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'personId',

            [ria.templates.ModelPropertyBind],
            String, 'displayName',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.ClassId, 'classId',

            [ria.templates.ModelPropertyBind],
            String, 'classNumber',

            [ria.templates.ModelPropertyBind],
            String, 'className',

            function getRecipientText(){
                return this.getDisplayName() || (this.getClassNumber() + ' ' + this.getClassName());
            },

            function getRecipientValue(){
                var personId = this.getPersonId() ? this.getPersonId().valueOf() : null;
                var classId = this.getClassId() ? this.getClassId().valueOf() : null;
                if(!personId && !classId)
                    return '';
                return JSON.stringify([personId, classId]);
            }
        ])
});

