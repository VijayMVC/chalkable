REQUIRE('chlk.activities.TemplateActivity');

NAMESPACE('chlk.activities', function () {

    /** @class chlk.activities.BindTemplate */

    var UNDER_OVERLAY_CLASS = 'under-overlay';
    var HIDDEN_CLASS = 'x-hidden';

    /** @class chlk.activities.TemplateDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('body')],
        'TemplateDialog', EXTENDS(chlk.activities.TemplateActivity), [
            function $() {
                BASE();
                this._overlay = new ria.dom.Dom('#chlk-overlay');
                this._dialogsHolder = new ria.dom.Dom('#chlk-dialogs');

                this._clickMeHandler = function () {
                    this.close();
                    return false;
                }.bind(this)
            },

            OVERRIDE, VOID, function onResume_() {
                BASE();
                this._dialogsHolder.removeClass(HIDDEN_CLASS);
                this._overlay
                    .removeClass(HIDDEN_CLASS)
                    .on('click', this._clickMeHandler);

                this.dom.removeClass(UNDER_OVERLAY_CLASS);
            },

            OVERRIDE, VOID, function onPause_() {
                this.dom.addClass(UNDER_OVERLAY_CLASS);
                this._dialogsHolder.addClass(HIDDEN_CLASS);
                this._overlay
                    .addClass(HIDDEN_CLASS)
                    .off('click', this._clickMeHandler);
                BASE();
            }
        ]);
});
