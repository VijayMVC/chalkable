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
            VOID, function checkImage(timeOut, img){
                var parent = img.parent();
                setTimeout(function(){
                    if(img.height() <= parent.height()/2 && img.width() <= img.parent().width()/2){
                        img.setCss('visibility', 'hidden');
                        parent.addClass('loading');
                        var src = img.getAttr('src');
                        img.setAttr('src', src);
                        this.checkImage(timeOut > 10 ? 10 : timeOut * 2, img);
                    }else{
                        img.setCss('visibility', 'visible');
                        parent.removeClass('loading');
                    }
                }.bind(this), timeOut * 100);
            },

            [[Object]],
            Object, function processAttrs(attrs){
                attrs.id = attrs.id || ria.dom.NewGID();
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        var img = new ria.dom.Dom('#' + attrs.id);
                        this.checkImage(1, img);
                    }.bind(this));
                return attrs;
            }
        ])
});