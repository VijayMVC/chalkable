REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.apps.AppInfo');
REQUIRE('chlk.templates.developer.DeveloperDocs');

NAMESPACE('chlk.activities.developer', function () {

    /** @class chlk.activities.developer.DeveloperDocsPage*/

    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.BodyClass('developer-docs')],
        [ria.mvc.TemplateBind(chlk.templates.developer.DeveloperDocs)],
        'DeveloperDocsPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            function $() {
                BASE();
                this._frameResizeHandler = function () {
                    jQuery('#dev-docs').height(jQuery(window).height() - 92);
                }.bind(this);

                jQuery(window).resize(this._frameResizeHandler);
            },

            OVERRIDE, VOID, function onRefresh_(data) {
                BASE(data);
                jQuery('#dev-docs').height(jQuery(window).height() - 92);
            }
        ]);
});