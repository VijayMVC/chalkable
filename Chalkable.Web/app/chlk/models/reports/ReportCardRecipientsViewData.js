REQUIRE('chlk.models.group.Group');

NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.ReportCardRecipientsViewData*/
    CLASS('ReportCardRecipientsViewData', [

        ArrayOf(chlk.models.group.Group), 'groups',

        [[ArrayOf(chlk.models.group.Group)]],
        function $(groups_){
            BASE();
            if(groups_)
                this.setGroups(groups_);
        }
    ]);
});
