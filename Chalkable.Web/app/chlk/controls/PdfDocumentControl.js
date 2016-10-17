REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.PdfDocumentControl */
    CLASS(
        'PdfDocumentControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/pdf.jade')(this);
            },

            [[Object]],
            Object, function processAttrs(attributes) {
                attributes.id = attributes.id || ria.dom.Dom.GID();
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        PDFObject.embed(attributes['data-url'], "#" + attributes.id);
                    }.bind(this));
                return attributes;
            }
        ]);
});
