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
                var alternativeSrc = node.attr('alternativeSrc');
                var defaultSrc = node.attr('defaultSrc');
                var needAlternative = node.attr('src') != alternativeSrc;
                var src = needAlternative ? alternativeSrc : defaultSrc;
                node.attr('src', src);
                if(!(needAlternative && defaultSrc))
                    node.off('error.load');
            },

            function canShowAlert(person){
                var user = this.getContext().getSession().get(ChlkSessionConstants.CURRENT_PERSON, null);
                if(user.getRole().getName().toLowerCase() ==  'student' && user.getId() != person.getId())
                    return false;
                return true;
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