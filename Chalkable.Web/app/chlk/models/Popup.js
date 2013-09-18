NAMESPACE('chlk.models', function () {
    "use strict";
    /** @class chlk.models.Popup*/
    CLASS(
        'Popup', [
            ria.dom.Dom, 'target',
            ria.dom.Dom, 'container',

            [[ria.dom.Dom, ria.dom.Dom]],
            function $(target, container){
                BASE();
                this.setTarget(target);
                this.setContainer(container);
            }
        ]);
});
