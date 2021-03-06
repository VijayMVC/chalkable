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

            chlk.models.id.ClassId, 'classId',

            Boolean, 'byLastName',

            Boolean, 'hasAccessToLE',

            Boolean, 'ableChangeReasons',

            Boolean, 'inProfile',

            [[chlk.models.classes.ClassesForTopBar, chlk.models.id.ClassId, ArrayOf(chlk.models.attendance.ClassAttendance),
                chlk.models.common.ChlkDate, Boolean,  ArrayOf(chlk.models.attendance.AttendanceReason), Boolean, Boolean, Boolean, Boolean, Boolean]],
            function $(topData_, selectedId_, items_, date_, byLastName_, reasons_, canRePost_, canSetAttendance_, canChangeReasons_, hasAccessToLE_, inProfile_){
                BASE(topData_, selectedId_);
                if(selectedId_)
                    this.setClassId(selectedId_);
                if(inProfile_)
                    this.setInProfile(inProfile_);
                if(items_)
                    this.setItems(items_);
                if(date_)
                    this.setDate(date_);
                if(reasons_)
                    this.setReasons(reasons_);
                if(byLastName_)
                    this.setByLastName(byLastName_);
                if(hasAccessToLE_)
                    this.setHasAccessToLE(hasAccessToLE_);
                if(canChangeReasons_)
                    this.setAbleChangeReasons(canChangeReasons_);

                    this._canRePost = !!canRePost_;
                    this._canSetAttendance = !!canSetAttendance_;
            },

            READONLY, Boolean, 'posted',
            Boolean, function isPosted(){
                var items = this.getItems();
                return items && items.length > 0
                    && items.filter(function(item){return item.isPosted()}).length > 0;
            },

            READONLY, Boolean, 'readOnly',
            Boolean, function isReadOnly(){
                var items = this.getItems();
                return items && items.length > 0
                    && items.filter(function(item){ return item.isReadOnly()}).length > 0;
            },

            READONLY, String, 'readOnlyReason',
            String, function getReadOnlyReason(){
                var items = this.getItems();
                return items && items.length > 0 ? items[0].getReadOnlyReason() : null;
            },

            Boolean, function canPost(){
                return !!this._canSetAttendance && !this.isReadOnly() && (!this.isPosted() || !!this._canRePost);
            }
        ]);
});
