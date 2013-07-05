REQUIRE('chlk.activities.lib.TemplateActivity');

NAMESPACE('chlk.activities.lib', function () {

    /** @class chlk.activities.lib.TemplatePopup */

    var HIDDEN_CLASS = 'x-hidden';
    var positionClasses = {
        top: 'popup-top',
        left: 'popup-left',
        right: 'popup-right',
        bottom: 'popup-bottom'
    };

    /** @class chlk.activities.lib.IsHorizontalAxis */
    ANNOTATION(
        [[Boolean]],
        function IsHorizontalAxis(isHorizontal) {});

    /** @class chlk.activities.lib.isTopLeftPosition */
    ANNOTATION(
        [[Boolean]],
        function isTopLeftPosition(isTopLeft) {});

    /** @class chlk.activities.lib.TemplatePopup*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-pop-up-container')],
        'TemplatePopup', EXTENDS(chlk.activities.lib.TemplateActivity), [
            function $() {
                BASE();
                this._body = new ria.dom.Dom('body');
                this.setContainer(this._body);
                this._popupHolder = new ria.dom.Dom('#chlk-pop-up-container');
                this._clickMeHandler = function(target, event){
                    if(!this._popupHolder.areEquals(target) && !this._popupHolder.contains(target))
                        this.close();
                }.bind(this)
            },

            ria.dom.Dom, 'target',

            ria.dom.Dom, 'container',

            String, 'currentClass',

            OVERRIDE, VOID, function bind_() {
                BASE();
                var ref = ria.reflection.ReflectionFactory(this.getClass());
                if (ref.isAnnotatedWith(chlk.activities.lib.IsHorizontalAxis)){
                    this._isHorizontal = ref.findAnnotation(chlk.activities.lib.IsHorizontalAxis)[0].isHorizontal;
                }else{
                    throw new ria.mvc.MvcException('There is no chlk.activities.lib.IsHorizontalAxis annotation for Popup activity');
                }
                this._isTopLeft = ref.isAnnotatedWith(chlk.activities.lib.isTopLeftPosition) ? ref.findAnnotation(chlk.activities.lib.isTopLeftPosition)[0].isTopLeft : true;
            },

            OVERRIDE, VOID, function onResume_() {
                BASE();
                this._body.on('click', this._clickMeHandler);
            },

            [[Object]],
            OVERRIDE, VOID, function onRender_(model) {
                BASE(model);
                var target = model.getTarget();
                var offset = target.offset();
                this._popupHolder = model.getContainer() || this._popupHolder;
                this._popupHolder.removeClass(HIDDEN_CLASS);
                if(!target)
                    throw new ria.mvc.MvcException('There is no target for Popup activity');
                var container = this.getContainer(), res;
                if(this._isHorizontal){
                    this._popupHolder.setCss('top', offset.top - 10);
                    if(this._isTopLeft && (container.offset().left + container.width()) > (this._popupHolder.offset().left + this._popupHolder.width())){
                        this._popupHolder.setCss('left', offset.left - 10 - this._popupHolder.width());
                        res = positionClasses.left;
                    }else{
                        this._popupHolder.setCss('left', offset.left + 10 + target.width());
                        res = positionClasses.right;
                    }
                }else{
                    this._popupHolder.setCss('left', offset.left - (this._popupHolder.width() - target.width())/2);
                    if(this._isTopLeft && (container.offset().top + container.height()) > (this._popupHolder.offset().top + this._popupHolder.height())){
                        this._popupHolder.setCss('top', offset.top - 10 - this._popupHolder.height());
                        res = positionClasses.top;
                    }else{
                        this._popupHolder.setCss('top', offset.top + 10 + target.height());
                        res = positionClasses.bottom;
                    }
                }
                this._popupHolder.addClass(res);
                this.setCurrentClass(res);
            },

            OVERRIDE, VOID, function onPause_() {
                this._popupHolder.addClass(HIDDEN_CLASS);
                this._popupHolder.removeClass(this.getCurrentClass());
                this._body.off('click', this._clickMeHandler);
                BASE();
            }
        ]);
});
