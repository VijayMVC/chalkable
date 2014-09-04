REQUIRE('chlk.activities.lib.ChlkTemplateActivity');

NAMESPACE('chlk.activities.lib', function () {

    var UNDER_OVERLAY_CLASS = 'under-overlay';
    var HIDDEN_CLASS = 'x-hidden';
    var INNER_PARTIAL_UPDATE_CLASS = "partial-update-inner";

    /** @class chlk.activities.lib.TemplateDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('body')],
        'TemplateDialog', EXTENDS(chlk.activities.lib.ChlkTemplateActivity), [
            function $() {
                BASE();
                this._overlay = new ria.dom.Dom('#chlk-overlay');
                this._dialogsHolder = new ria.dom.Dom('#chlk-dialogs');
            },

            OVERRIDE, VOID, function onResume_() {
                BASE();
                this._dialogsHolder.removeClass(HIDDEN_CLASS);
                this._overlay
                    .removeClass(HIDDEN_CLASS)
                    .on('click.overlay', function clickMeHandler(node, event) {
                        this.close();
                        return false;
                    }.bind(this));

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
                BASE();
            },

            [ria.mvc.DomEventBind('click', '.close')],
            [[ria.dom.Dom, ria.dom.Event]],
            Boolean, function onCloseBtnClick(node, event) {
                this.close();
                return false;
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
