REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.apps.Apps');

NAMESPACE('chlk.activities.apps', function () {

    /** @class chlk.activities.apps.AppsListPage*/
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.apps.Apps)],
        'AppsListPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('change', 'input[name=isInternal]')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function isInternalChanged(node, event){
                var isInternal = node.checked();
                node.parent().trigger('submit');
            },

            [ria.mvc.DomEventBind('change', '#developers-select, #states-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function sortingChanged(node, event, data){
//                this.resetScrolling_();

                this.dom.find('#apps-list-form').trigger('submit');
            }
        ]);
});