REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.LoadingImgControl */
    CLASS(
        'LoadingImgControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/loading-img.jade')(this);
            },

            [[Number, Object]],
            VOID, function checkImage(timeOut, img, attrs){
                var parent = img.parent();
                var timer = setTimeout(function(){
                    img.css('visibility', 'hidden');
                    parent.addClass('loading');
                    var src = img.attr('src');
                    img.attr('src', src);
                    this.checkImage(timeOut > 10 ? 10 : timeOut * 2, img, attrs);
                }.bind(this), timeOut * 100);
                img.data('timer', timer);
            },

            [[Object]],
            Object, function processAttrs(attrs){
                attrs.id = attrs.id || ria.dom.Dom.GID();
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        var that = this, img = jQuery('#' + attrs.id);
                        img.on('error', function(){
                            that.checkImage(1, img, attrs);
                        });

                        img.on('load', function(){
                            img.css('visibility', 'visible');
                            img.parent().removeClass('loading');
                            clearTimeout(img.data('timer'));
                        });

                    }.bind(this));
                return attrs;
            }
        ])
});