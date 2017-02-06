REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.recipients.GroupsListViewData');
REQUIRE('chlk.models.recipients.BaseViewData');

NAMESPACE('chlk.templates.controls.group_people_selector', function () {

    /** @class chlk.templates.controls.group_people_selector.GroupsListTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/controls/group-people-selector/groups-list.jade')],
        [ria.templates.ModelBind(chlk.models.recipients.GroupsListViewData)],
        'GroupsListTpl', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            String, 'filter',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.group.Group), 'groups',

            Array, 'selected',

            chlk.models.recipients.SelectorModeEnum, 'selectorMode',

            Number, 'allCount',

            function getCountText() {
                var currentCount = this.getGroups().length, allCount = this.getAllCount();

                var res = currentCount;
                
                if(allCount > currentCount)
                    res += ' of ' + allCount;

                res += ' Group' + (currentCount == 1 ? "" : "s");
                return res;
            }
        ])
});