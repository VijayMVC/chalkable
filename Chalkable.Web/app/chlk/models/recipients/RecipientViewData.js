NAMESPACE('chlk.models.recipients', function () {
    "use strict";

    /** @class chlk.models.recipients.RecipientViewData*/
    CLASS(
        'RecipientViewData',  [
            Boolean, 'isGroup',
            String, 'gender',
            String, 'name',
            Object, 'id'
        ]);
});
