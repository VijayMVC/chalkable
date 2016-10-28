REQUIRE('chlk.models.group.Group');

NAMESPACE('chlk.models.reports', function () {
    "use strict";

    /** @class chlk.models.reports.ReportCardRecipientsViewData*/
    CLASS('ReportCardRecipientsViewData', [

        ArrayOf(chlk.models.group.Group), 'groups',

        Object, 'selectedItems',
        
        ArrayOf(chlk.models.people.ShortUserInfo), 'students',

        [[ArrayOf(chlk.models.group.Group), ArrayOf(chlk.models.people.ShortUserInfo), Object]],
        function $(groups_, students_, selectedItems_){
            BASE();
            if(groups_)
                this.setGroups(groups_);
            if(students_)
                this.setStudents(students_);
            if(selectedItems_)
                this.setSelectedItems(selectedItems_);
        }
    ]);
});
