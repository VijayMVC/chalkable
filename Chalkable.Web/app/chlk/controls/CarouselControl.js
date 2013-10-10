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
            VOID, function updateDot(target){
                jQuery('.carousel-pagination .dot.selected').removeClass('selected');
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

                if (index > 0)
                    index -= 1;
                jQuery('.carousel-pagination .dot:eq(' + index + ')').addClass('selected');


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
                    that.updateDot(target);
                    return false;
                });

                jQuery('.picture-right').click(function(event){
                    jQuery('.carousel').jcarousel('scroll', '+=1');
                    var target = jQuery('.carousel').jcarousel('target');
                    that.updateDot(target);
                    return false;
                });


                jQuery('.carousel-pagination').jcarouselPagination({
                    item: function(page, carouselItems) {
                        return '<div class="dot" data-page="' + page + '"></div>';
                    }
                });

                jQuery('.carousel-pagination .dot').click(function(item){
                    var index = jQuery(this).data('page');
                    if (index > 0) index -= 1;

                    var newElem = jQuery('.carousel li:eq(' + index + ')');
                    jQuery('.carousel').jcarousel('scroll', newElem);

                    that.updateDot(newElem);
                    return false;
                });

                jQuery('.carousel-pagination .dot:first').addClass('selected');

            }

        ]);
});