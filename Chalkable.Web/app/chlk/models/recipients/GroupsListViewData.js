REQUIRE('chlk.models.group.Group');
REQUIRE('chlk.models.recipients.BaseViewData');

NAMESPACE('chlk.models.recipients', function () {
    "use strict";

    /** @class chlk.models.recipients.GroupsListViewData*/
    CLASS(
        'GroupsListViewData', EXTENDS(chlk.models.recipients.BaseViewData), [
            String, 'filter',
            ArrayOf(chlk.models.group.Group), 'groups',
            Number, 'start',
            Number, 'count',
            String, 'submitType',

            [[ArrayOf(chlk.models.group.Group), String]],
            function $(groups_, filter_){
                BASE();
                groups_ && this.setGroups(groups_);
                filter_ && this.setFilter(filter_);
            }
        ]);
});
