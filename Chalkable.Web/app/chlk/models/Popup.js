NAMESPACE('chlk.models', function () {
    "use strict";
    /** @class chlk.models.Popup*/
    CLASS(
        'Popup', [
            ria.dom.Dom, 'target',
            ria.dom.Dom, 'container',

            [[ria.dom.Dom, ria.dom.Dom]],
            function $(target_, container_){
                BASE();
                if(target_)
                    this.setTarget(target_);
                if(container_)
                    this.setContainer(container_);

            }
        ]);
});
