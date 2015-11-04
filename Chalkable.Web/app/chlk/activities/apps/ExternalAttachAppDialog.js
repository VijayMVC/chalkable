REQUIRE('chlk.activities.apps.AppWrapperDialog');
REQUIRE('chlk.templates.apps.ExternalAttachAppDialogTpl');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.ExternalAttachAppDialog*/
    CLASS(
        [ria.mvc.ActivityGroup('AttachDialog')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.apps.ExternalAttachAppDialogTpl)],
        'ExternalAttachAppDialog', EXTENDS(chlk.activities.apps.AppWrapperDialog), [

            [[Object]],
            OVERRIDE, VOID, function onRefresh_(data) {
                BASE(data);
                this.dom.find('iframe').$.load(function () {
                    this.unfreeze(null, null)
                }.bind(this))
            }
        ]);
});
