REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.LoadingImgControl */
    CLASS(
        'LoadingImgControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/loading-img.jade')(this);
            },


            [[Object]],
            Object, function processAttrs(attrs){
                attrs.id = attrs.id || ria.dom.Dom.GID();
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        var img = jQuery('#' + attrs.id),
                            src = img.attr('src');

                        img
                            .on('error', function(){
                                img.css('visibility', 'hidden');
                                var timeout = img.data('img-loader.timeout') || 50,
                                    retries = img.data('img-loader.retries') || 0;

                                img.parent().addClass('loading');

                                (retries < 25) && setTimeout(function () {
                                    img.attr('src', src)
                                        .data('img-loader.timeout', Math.min(timeout * 2 + Math.random() * 100, 10000) - Math.random() * 50)
                                        .data('img-loader.retries', retries+1);
                                }, timeout);
                            })

                            .on('load', function(){
                                img.css('visibility', 'visible');
                                img.parent().removeClass('loading');
                                clearTimeout(img.data('timer'));
                            });

                    }.bind(this));
                return attrs;
            }
        ])
});
