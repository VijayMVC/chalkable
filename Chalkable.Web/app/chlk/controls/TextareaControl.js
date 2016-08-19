REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    var autosize = window.autosize,
        document = window.document;

    /** @class chlk.controls.TextAreaControl */
    CLASS(
        'TextAreaControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/textarea.jade')(this);
            },

            [[Object]],
            Object, function processAttrs(attributes) {
                attributes.id = attributes.id || ria.dom.Dom.GID();
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        autosize(document.getElementById(attributes.id));
                    }.bind(this));
                return attributes;
            }
        ]);
});
