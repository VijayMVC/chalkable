REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.CarouselControl */
    CLASS(
        'CarouselControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/carousel.jade')(this);
            },


            [[Object]],
            Object, function processAttrs(attributes) {
                attributes.id = attributes.id || ria.dom.NewGID();
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        this.update(jQuery('#'+attributes.id));
                    }.bind(this));
                return attributes;
            },

            [[Object]],
            VOID, function update(node){
                node.jcarousel({
                    itemFallbackDimension: 640,
                    // Configuration goes here
                    buttonNextHTML: '<div class="picture-button picture-right"></div>',
                    buttonPrevHTML: '<div class="picture-button picture-left"></div>'
                });
                //jQuery('.carousel-pagination').jcarouselPagination();

            }
        ]);
});