REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.apps.InstallAppDialogTpl');

NAMESPACE('chlk.activities.apps', function () {
    /** @class chlk.activities.apps.InstallAppDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.apps.InstallAppDialogTpl)],
        'InstallAppDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [

            [ria.mvc.DomEventBind('click', '.chlk-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function submitClick(node, event){

                //regroup checkboxes make viewdata and send
                //var addressesNode = this.dom.find('input[name="addressesValue"]');
                //var phonesNode = this.dom.find('input[name="phonesValue"]');
                //addressesNode.setValue(JSON.stringify(this.getAddresses()));
                //phonesNode.setValue(JSON.stringify(this.getPhones()));
            }
        ]);
});