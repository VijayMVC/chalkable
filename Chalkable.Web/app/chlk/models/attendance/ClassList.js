REQUIRE('chlk.models.attendance.ClassAttendance');
REQUIRE('chlk.models.common.PageWithClasses');
REQUIRE('chlk.models.common.ChlkDate');
REQUIRE('chlk.models.attendance.AttendanceReason');

NAMESPACE('chlk.models.attendance', function () {
    "use strict";

    /** @class chlk.models.attendance.ClassList*/
    CLASS(
        'ClassList', EXTENDS(chlk.models.common.PageWithClasses), [
            ArrayOf(chlk.models.attendance.ClassAttendance), 'items',

            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',

            chlk.models.common.ChlkDate, 'date',

            Boolean, 'byLastName',


            [[chlk.models.classes.ClassesForTopBar, chlk.models.id.ClassId, ArrayOf(chlk.models.attendance.ClassAttendance), chlk.models.common.ChlkDate,
                Boolean,  ArrayOf(chlk.models.attendance.AttendanceReason), Boolean, Boolean]],
            function $(topData_, selectedId_, items_, date_, byLastName_, reasons_, canRePost_, canSetAttendance_){
                BASE(topData_, selectedId_);
                if(items_)
                    this.setItems(items_);
                if(date_)
                    this.setDate(date_);
                if(reasons_)
                    this.setReasons(reasons_);
                if(byLastName_)
                    this.setByLastName(byLastName_);

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
