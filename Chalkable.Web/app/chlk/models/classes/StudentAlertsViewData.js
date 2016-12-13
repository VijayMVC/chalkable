REQUIRE('chlk.interfaces.IPopup');
REQUIRE('chlk.models.people.ShortUserInfo');

NAMESPACE('chlk.models.classes', function () {
    "use strict";

    /** @class chlk.models.classes.StudentAlertsViewData*/
    CLASS(
        'StudentAlertsViewData', EXTENDS(chlk.models.people.ShortUserInfo), IMPLEMENTS(ria.serialize.IDeserializable, chlk.interfaces.IPopup), [
            ria.dom.Dom, 'target',
            ria.dom.Dom, 'container',

            OVERRIDE, VOID, function deserialize(raw) {
                BASE(raw);
            }
        ]);
});
