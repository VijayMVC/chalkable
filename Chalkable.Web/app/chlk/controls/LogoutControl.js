REQUIRE('chlk.controls.Base');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.LogoutControl*/
    CLASS(
        'LogoutControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/log-out.jade')(this);
                this.setLogoutShown(false);
            },


            Boolean, 'logoutShown',

            [ria.mvc.DomEventBind('click', '.logout-area')],
            [[ria.dom.Dom, ria.dom.Event]],
            function onClicked($target, event) {
                var elem = $target.parent().find('.logout');
                if(!this.isLogoutShown()){
                    elem.setCss("visibility", "visible")
                        .setCss("opacity", 1)
                        .setCss("height", "auto");
                }else{
                    elem.setCss("opacity", 0).setCss("height", 0);
                    setTimeout(function(){
                        elem.setCss("visibility", "hidden").setCss('height', 0);
                    }, 200);
                }
                this.setLogoutShown(!this.isLogoutShown());
            },

            [ria.mvc.DomEventBind('click', 'BODY')],
            [[ria.dom.Dom, ria.dom.Event]],
            function onBodyClicked($target, event) {
                var $parent = ria.dom.Dom('header .logout-area').parent();

                if (!this.isLogoutShown() || $parent.contains(ria.dom.Dom(event.target)))
                    return ;

                var elem = $parent.find('.logout');
                elem.setCss("opacity", 0).setCss("height", 0);
                setTimeout(function(){
                    elem.setCss("visibility", "hidden");
                }, 200);
                this.setLogoutShown(false);
            }
    ])
});