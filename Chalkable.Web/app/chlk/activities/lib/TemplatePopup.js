REQUIRE('chlk.activities.lib.ChlkTemplateActivity');

NAMESPACE('chlk.activities.lib', function () {

    /** @class chlk.activities.lib.TemplatePopup */

    var HIDDEN_CLASS = 'x-hidden';
    var positionClasses = {
        top: 'popup-top',
        left: 'popup-left',
        right: 'popup-right',
        bottom: 'popup-bottom'
    };

    var bubbleClasses = {
        top: 'top',
        left: 'left',
        right: 'right',
        bottom: 'bottom'
    };


    /** @class chlk.activities.lib.PopupClass */
    ANNOTATION(
        [[String]],
        function PopupClass(clazz) {});


    /** @class chlk.activities.lib.IsHorizontalAxis */
    ANNOTATION(
        [[Boolean]],
        function IsHorizontalAxis(isHorizontal) {});

    /** @class chlk.activities.lib.isTopLeftPosition */
    ANNOTATION(
        [[Boolean]],
        function isTopLeftPosition(isTopLeft) {});

    /** @class chlk.activities.lib.isConstantPosition */
    ANNOTATION(
        [[Boolean]],
        function isConstantPosition(isConstant) {});

    /** @class chlk.activities.lib.TemplatePopup*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-pop-up-container')],
        'TemplatePopup', EXTENDS(chlk.activities.lib.ChlkTemplateActivity), [
            function $() {
                BASE();
                this._body = new ria.dom.Dom('body');
                this.setContainer(this._body);
                this._popupHolder = new ria.dom.Dom('#chlk-pop-up-container');
                this._clickMeHandler = function(node, event){
                    var target = new ria.dom.Dom(event.target);
                    if(!this._popupHolder.equals(target) && !this._popupHolder.contains(target))
                        this.close();
                }.bind(this)
            },

            ria.dom.Dom, 'target',

            ria.dom.Dom, 'container',

            String, 'currentClass',

            OVERRIDE, VOID, function processAnnotations_(ref) {
                BASE(ref);
                if (ref.isAnnotatedWith(chlk.activities.lib.IsHorizontalAxis)){
                    this._isHorizontal = ref.findAnnotation(chlk.activities.lib.IsHorizontalAxis)[0].isHorizontal;
                }else{
                    throw new ria.mvc.MvcException('There is no chlk.activities.lib.IsHorizontalAxis annotation for Popup activity');
                }
                this._isTopLeft = ref.isAnnotatedWith(chlk.activities.lib.isTopLeftPosition) ? ref.findAnnotation(chlk.activities.lib.isTopLeftPosition)[0].isTopLeft : true;

                this._isConstantPosition = ref.isAnnotatedWith(chlk.activities.lib.isConstantPosition) ? ref.findAnnotation(chlk.activities.lib.isConstantPosition)[0].isConstant : false;

                this._popupClass = null;
                if(ref.isAnnotatedWith(chlk.activities.lib.PopupClass)){
                    this._popupClass = ref.findAnnotation(chlk.activities.lib.PopupClass)[0].clazz;
                }
            },

            OVERRIDE, VOID, function onResume_() {
                BASE();
                this._body.on('click.body', this._clickMeHandler);
            },

            [[Object]],
            OVERRIDE, VOID, function onRefresh_(model) {
                BASE(model);
                var target = model.getTarget();
                Assert(target, 'There is no target for Popup activity');
                var offset = target.offset(), bubbleClass;
                if(offset){
                    this._popupHolder = model.getContainer() || this._popupHolder;
                    this._popupClass && this._popupHolder.addClass(this._popupClass);
                    this._popupHolder.removeClass(HIDDEN_CLASS);
                    var container = this._popupHolder;
                    if(!this._popupHolder.height() && this._popupHolder.find('.popup-bubble').exists())
                        container = this._popupHolder.find('.popup-bubble');

                    var popupWidth = container.width() + parseInt(container.getCss('padding-left'), 10) + parseInt(container.getCss('padding-right'), 10),
                        popupHeight = container.height() + parseInt(container.getCss('padding-top'), 10) + parseInt(container.getCss('padding-bottom'), 10);

                    var bubble = this._popupHolder.find('.popup-bubble:eq(0)');


                    var container = this.getContainer(), res;
                    if(this._isHorizontal){
                        this._popupHolder.setCss('top', offset.top - 10);
                        var notOver = offset.left - container.offset().left > popupWidth;
                        if(this._isTopLeft && (notOver || this._isConstantPosition)){
                        //if(this._isTopLeft && (container.offset().left + container.width()) > (this._popupHolder.offset().left + this._popupHolder.width())){
                            this._popupHolder.setCss('left', notOver ? offset.left - 10 - popupWidth : 0);
                            res = positionClasses.left;
                            bubbleClass = bubbleClasses.right;
                        }else{
                            this._popupHolder.setCss('left', offset.left + 10 + target.width());
                            res = positionClasses.right;
                            bubbleClass = bubbleClasses.left;
                        }
                    }else{
                        if(popupWidth > target.width())
                            this._popupHolder.setCss('left', offset.left);
                        else
                            this._popupHolder.setCss('left', offset.left - (popupWidth - target.width())/2);
                        var notOver = offset.top - container.offset().top > popupHeight;
                        if(this._isTopLeft && (notOver || this._isConstantPosition)){
                        //if(this._isTopLeft && (container.offset().top + container.height()) > (this._popupHolder.offset().top + this._popupHolder.height())){

                            this._popupHolder.setCss('top', notOver ? offset.top - 10 - popupHeight : 0);
                            res = positionClasses.top;
                            bubbleClass = bubbleClasses.bottom;
                        }else{
                            this._popupHolder.setCss('top', offset.top + 10 + target.height());
                            res = positionClasses.bottom;
                            bubbleClass = bubbleClasses.top;
                        }
                    }
                    this._popupHolder.addClass(res);
                    this.setCurrentClass(res);
                    bubble.removeClass(bubbleClasses.top);
                    bubble.removeClass(bubbleClasses.bottom);
                    bubble.removeClass(bubbleClasses.left);
                    bubble.removeClass(bubbleClasses.right);
                    bubble.addClass(bubbleClass);
                }

            },

            OVERRIDE, VOID, function onPause_() {
                this._popupHolder.addClass(HIDDEN_CLASS);
                this._popupHolder.removeClass(this.getCurrentClass());
                this._popupClass && this._popupHolder.removeClass(this._popupClass);
                this._body.off('click.body', this._clickMeHandler);
                BASE();
            }
        ]);
});
