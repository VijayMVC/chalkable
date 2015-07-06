REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.apps.AddAppDialog');

NAMESPACE('chlk.activities.apps', function () {

    var HIDDEN_CLASS = 'x-hidden';
    var WIDE_CLASS = 'wide';
    var TOP_CLASS = 'top';
    /** @class chlk.activities.apps.AddAppDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.apps.AddAppDialog)],
        'AddAppDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [


            function $(){
                BASE();
                this._appsCombo = new ria.dom.Dom('#dev-apps-list');
                this._demoFooter = new ria.dom.Dom('#demo-footer');
                this._devApps = new ria.dom.Dom('#dev-apps');

            },
            OVERRIDE, VOID, function onResume_() {
                BASE();
                this._appsCombo.addClass(HIDDEN_CLASS);
                this._devApps.addClass(WIDE_CLASS);
                this._demoFooter.removeClass(TOP_CLASS);
            },

            OVERRIDE, VOID, function onPause_() {
                this._appsCombo.removeClass(HIDDEN_CLASS);
                this._devApps.removeClass(WIDE_CLASS);
                this._demoFooter.addClass(TOP_CLASS);
                BASE();
            }
        ]);
});