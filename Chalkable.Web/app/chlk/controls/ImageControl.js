REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.ImageControl */
    CLASS(
        'ImageControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/image.jade')(this);
            },

            [[Object]],
            VOID, function onImgError(event){
                var node = jQuery(event.target);
                var needAlternative = node.attr('src') != this.getAlternativeSrc();
                var src = needAlternative ? this.getAlternativeSrc() : this.getDefaultSrc();
                node.attr('src', src);
                if(!(needAlternative && this.getDefaultSrc()))
                    node.off('error.load');
            },

            String, 'alternativeSrc',

            String, 'defaultSrc',

            [[Object]],
            Object, function processAttrs(attributes){
                attributes.id = attributes.id || ria.dom.NewGID();
                if(!attributes.alternativeSrc && attributes.defaultSrc){
                    attributes.alternativeSrc = attributes.defaultSrc;
                    delete attributes.defaultSrc;
                }
                attributes.alternativeSrc && this.setAlternativeSrc(attributes.alternativeSrc);
                attributes.defaultSrc && this.setDefaultSrc(attributes.defaultSrc);
                if(attributes.alternativeSrc)
                    this.context.getDefaultView()
                        .onActivityRefreshed(function (activity, model) {
                            jQuery('#'+attributes.id).on('error.load', this.onImgError);
                        }.bind(this));
                return attributes;
            }
        ]);
});