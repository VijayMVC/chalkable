REQUIRE('chlk.activities.lib.ChlkTemplateActivity');

NAMESPACE('chlk.activities.lib', function () {

    var UNDER_OVERLAY_CLASS = 'under-overlay';
    var HIDDEN_CLASS = 'x-hidden';
    var INNER_PARTIAL_UPDATE_CLASS = "partial-update-inner";
    var STOP_SCROLLING_CLASS = 'stop-scrolling';

    /** @class chlk.activities.lib.FixedTop */
    ANNOTATION(
        function FixedTop(top_) {});

    /** @class chlk.activities.lib.TemplateDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('body')],
        'TemplateDialog', EXTENDS(chlk.activities.lib.ChlkTemplateActivity), [
            function $() {
                this._fixedTopOffest = false;

                BASE();
                this._overlay = new ria.dom.Dom('#chlk-overlay');
                this._dialogsHolder = new ria.dom.Dom('#chlk-dialogs');
            },

            [[ria.reflection.ReflectionClass]],
            OVERRIDE, VOID, function processAnnotations_(ref) {
                BASE(ref);

                if (ref.isAnnotatedWith(chlk.activities.lib.FixedTop)) {
                    this._fixedTopOffest = ref.findAnnotation(chlk.activities.lib.FixedTop).pop().top_ || 50;
                }
            },

            OVERRIDE, VOID, function onStart_() {
                BASE();
                ria.dom.Dom('body').addClass(STOP_SCROLLING_CLASS);
            },

            function afterRefresh_() {
                if (this._fixedTopOffest !== false) {
                    this.dom.find('.dialog')
                        .setCss('top', ria.dom.Dom(document).scrollTop() + 80)
                        .addClass('fixed-top');

                    this._fixedTopOffest = false;
                }
            },

            [[Object, String]],
            OVERRIDE, VOID, function onPartialRefresh_(model, msg_){
                BASE(model, msg_);
                this.afterRefresh_();
            },

            [[Object]],
            OVERRIDE, VOID, function onRefresh_(model){
                BASE(model);
                this.afterRefresh_();
            },

            OVERRIDE, VOID, function onResume_() {
                BASE();
                ria.dom.Dom('body').addClass(STOP_SCROLLING_CLASS);
                this._dialogsHolder.removeClass(HIDDEN_CLASS);
                this._overlay
                    .removeClass(HIDDEN_CLASS)
                    .on('click.overlay', this.onCloseBtnClick.bind(this));
                this._dialogsHolder.on('click.overlay', this.onDialogClick.bind(this));
                this.dom.removeClass(UNDER_OVERLAY_CLASS);
            },

            OVERRIDE, VOID, function onPause_() {

                var count = this._dialogsHolder.count();
                if (count == 1)
                    this._dialogsHolder.addClass(HIDDEN_CLASS);

                this.dom.addClass(UNDER_OVERLAY_CLASS);

                this._overlay
                    .addClass(HIDDEN_CLASS)
                    .off('click.overlay');

                this._dialogsHolder.off('click.overlay');

                ria.dom.Dom('body').removeClass(STOP_SCROLLING_CLASS);

                BASE();
            },

            function onDialogClick(node, event) {
                if(ria.dom.Dom(event.target).is('#chlk-dialogs>DIV')){
                    return this.tryClose_();
                }
            },

            function tryClose_(){
                var result = this.isReadyForClosing();

                if (result === true)
                    this.close();

                if (result instanceof ria.async.Future) {
                    result.then(function (yes) {
                        if (yes === true)
                            this.close();
                    }, this)
                }

                return false;
            },

            [ria.mvc.DomEventBind('click', '.close')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function onCloseBtnClick(node, event) {
                return this.tryClose_();
            },

            [ria.mvc.DomEventBind('click', '.disable-on-click')],
            [[ria.dom.Dom, ria.dom.Event]],
            function disableOnClick(node, event) {
                node.setAttr('disabled', 'disabled');
            },

            OVERRIDE, VOID, function addPartialRefreshLoader(msg_) {
                //todo: rewrite
                jQuery(this.dom.valueOf()).children().first().addClass(this._partialUpdateCls + "-" +INNER_PARTIAL_UPDATE_CLASS);
                BASE(msg_);
            },

            [[String]],
            OVERRIDE, VOID, function onModelComplete_(msg_) {
                //todo: rewrite
                jQuery(this.dom.valueOf()).children().first().removeClass(this._partialUpdateCls + "-" +INNER_PARTIAL_UPDATE_CLASS);
                BASE(msg_);
            }

        ]);
});
