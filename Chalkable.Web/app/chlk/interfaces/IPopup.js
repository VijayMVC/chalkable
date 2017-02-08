
NAMESPACE('chlk.interfaces', function () {
    "use strict";

    /** @class chlk.interfaces.IPopup */
    INTERFACE(
        'IPopup', [
            ria.dom.Dom, 'target',
            ria.dom.Dom, 'container',
        ]);
});