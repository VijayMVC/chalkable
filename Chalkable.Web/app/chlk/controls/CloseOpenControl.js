REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.CloseOpenControl*/
    CLASS(
        'CloseOpenControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/close-open.jade')(this);
            },

            VOID, function queueReanimation_(id) {
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        var node = ria.dom.Dom('#' + id),
                            container = node.find('.close-open-block'), timeout;
                        node.on('click', '.co-open, .co-close', function(){
                            clearTimeout(timeout);
                            if(node.hasClass('co-opened')){
                                container.setCss('height', 0);
                                node.removeClass('co-opened');
                            }
                            else{
                                container.setCss('height', 'auto');
                                node.addClass('co-opened');
                            }
                            timeout = setTimeout(function(){
                                node.addClass('co-finished');
                            }, 200)
                        })
                    }.bind(this));
            },

            [[Object]],
            Object, function processAttrs(attributes) {
                attributes.id = attributes.id || ria.dom.Dom.GID();
                this.queueReanimation_(attributes.id);
                return attributes;
            }
        ]);
});