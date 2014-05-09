REQUIRE('chlk.models.common.PageWithClasses');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.PostAttendanceViewData*/
    CLASS(
        'PostAttendanceViewData', EXTENDS(chlk.models.common.PageWithClasses), [
            [[chlk.models.classes.ClassesForTopBar, chlk.models.id.ClassId, chlk.models.common.ChlkDate,
                ArrayOf(chlk.models.attendance.AttendanceReason), Boolean, Boolean]],
            function $(topData_, selectedId_, date_, reasons_, canRePost_, canSetAttendance_){
                BASE(topData_, selectedId_);
                if(date_)
                    this.setDate(date_);
                if(reasons_)
                    this.setReasons(reasons_);
                this._canRePost = !!canRePost_;
                this._canSetAttendance = !!canSetAttendance_;
            },

            READONLY, Boolean, 'posted',
            Boolean, function isPosted(){
                var items = this.getItems();
                return items && items.length > 0
                    && items.filter(function(item){return item.isPosted()}).length > 0;
            },

            Boolean, function canPost(){
                return !!this._canSetAttendance  && (!this.isPosted() || !!this._canRePost);
            }

        ]);
});
