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
                var img = jQuery(event.target);
                var alternativeSrc = img.attr('alternativeSrc');
                var defaultSrc = img.attr('defaultSrc');
                var needAlternative = img.attr('src') != alternativeSrc;
                var src = needAlternative ? alternativeSrc : defaultSrc;

                if(!(needAlternative && defaultSrc))
                    img.off('error.load');

                var timeout = img.data('img-loader.timeout') || 50,
                    retries = img.data('img-loader.retries') || 0;

                img.parent().addClass('loading');

                (retries < 25) && setTimeout(function () {
                    img.attr('src', src)
                        .data('img-loader.timeout', Math.min(timeout * 2 + Math.random() * 100, 10000) - Math.random() * 50)
                        .data('img-loader.retries', retries+1);
                }, timeout);

            },

            function canShowAlert(person){
                var user = this.getContext().getSession().get(ChlkSessionConstants.CURRENT_PERSON, null);
                return !(user.getRole().getName().toLowerCase() == 'student' && user.getId() != person.getId());
            },

            String, 'alternativeSrc',

            String, 'defaultSrc',

            //todo: make base method
            [[Object]],
            Object, function processAttrs(attributes){
                attributes.id = attributes.id || ria.dom.Dom.GID();
                if(!attributes.alternativeSrc && attributes.defaultSrc){
                    attributes.alternativeSrc = attributes.defaultSrc;
                    delete attributes.defaultSrc;
                }
                //attributes.alternativeSrc && this.setAlternativeSrc(attributes.alternativeSrc);
                //attributes.defaultSrc && this.setDefaultSrc(attributes.defaultSrc);
                if(attributes.alternativeSrc)
                    this.context.getDefaultView()
                        .onActivityRefreshed(function (activity, model) {
                            jQuery('#'+attributes.id).on('error.load', this.onImgError);
                        }.bind(this));
                return attributes;
            },

            [[Object]],
            Object, function getAlertsAttributes(model){
                var len = 0, h = 123;
                if(model.isWithMedicalAlert())
                    len++;
                if(model.isAllowedInetAccess())
                    len++;
                if(model.getSpecialInstructions())
                    len++;
                if(model.getSpedStatus())
                    len++;
                var top = -(h * len) / 4;
                return top + 'px';
            }
        ]);
});
