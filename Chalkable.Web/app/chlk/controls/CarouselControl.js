REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.CarouselControl */
    CLASS(
        'CarouselControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/carousel.jade')(this);
            },


            Number, 'startPage',
            Number, 'lastPage',

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
            VOID, function updateControls(target){
                var index = target.data('page');

                if (index == this.getStartPage()){
                    jQuery('.picture-left').attr('disabled', true);
                } else {
                    jQuery('.picture-left').attr('disabled', false);
                }

                if (index == this.getLastPage()){
                    jQuery('.picture-right').attr('disabled', true);
                } else {
                    jQuery('.picture-right').attr('disabled', false);
                }
            },

            [[Object]],
            VOID, function update(node){
                node.jcarousel({
                    itemFallbackDimension: 640,
                    animation: 'fast',
                    'vertical': false
                });

                var pages = jQuery('.carousel li').map(function(){
                   return jQuery(this).data('page');
                }) || [];

                var startPage = Math.min.apply(null, pages) || 0;
                var lastPage = Math.max.apply(null, pages) || 0;

                this.setStartPage(startPage);
                this.setLastPage(lastPage);

                var that = this;

                jQuery('.picture-left').click(function(event){
                    jQuery('.carousel').jcarousel('scroll', '-=1');
                    var target = jQuery('.carousel').jcarousel('target');
                    that.updateControls(target);
                    return false;
                });

                jQuery('.picture-right').click(function(event){
                    jQuery('.carousel').jcarousel('scroll', '+=1');
                    var target = jQuery('.carousel').jcarousel('target');
                    that.updateControls(target);
                    return false;
                });

            }

        ]);
});