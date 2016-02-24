REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.bgtasks.BgTasks');

NAMESPACE('chlk.activities.bgtasks', function () {

    /** @class chlk.activities.bgtasks.BgTasksListPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.bgtasks.BgTasks)],
        'BgTasksListPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('change', '#types-select, #states-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function tasksFilteringChange(node, event, data){
                this.dom.find('#bgTasks-list-form').trigger('submit');
            },

            [ria.mvc.DomEventBind('change', '#districts-select')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function districtsChange(node, event, data){
                var allDistricts = !!node.find('option').filter(function(item){
                    return item.getText()== data.selected
                }).getData('allDistricts');
                this.dom.find('[name="isalldistricts"]').setValue(allDistricts);
                this.dom.find('#bgTasks-list-form').trigger('submit');
            }
        ]);
});