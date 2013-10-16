REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.apps.AppInfo');
REQUIRE('chlk.templates.developer.DeveloperDocs');

NAMESPACE('chlk.activities.developer', function () {

    /** @class chlk.activities.developer.DeveloperDocsPage*/


    var HIDDEN_CLS = 'x-hidden';
    CLASS(
        [ria.mvc.DomAppendTo('#content')],
        [chlk.activities.lib.BodyClass('developer-docs')],
        [ria.mvc.TemplateBind(chlk.templates.developer.DeveloperDocs)],
        'DeveloperDocsPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            function $() {
                BASE();
                this._iframe = new ria.dom.Dom('#dev-docs');
                this._demoFooter = new ria.dom.Dom('#demo-footer');
                this._window = jQuery(window);
                this._frameResizeHandler = function () {
                    this.setDefaultHeight();
                }.bind(this);
            },


            function setDefaultHeight(){
                //this._iframe.setCss('height', this._iframe.valueOf().contentWindow.document.height() - 92);
            },
            [[Object]],
            OVERRIDE, VOID, function onRender_(data) {
                BASE(data);

            },

            OVERRIDE, VOID, function onResume_() {
                BASE();
                this._demoFooter.addClass(HIDDEN_CLS);
                this._window.on('resize.frame', this._frameResizeHandler);
                this.setDefaultHeight();

            },

            OVERRIDE, VOID, function onPause_() {
                BASE();
                this._demoFooter.removeClass(HIDDEN_CLS);
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                this._window.off('resize.frame', this._frameResizeHandler);
            }

        ]);
});