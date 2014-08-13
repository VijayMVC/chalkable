REQUIRE('chlk.templates.common.PageWithClasses');
REQUIRE('chlk.models.attendance.ClassList');
REQUIRE('chlk.models.common.ActionLinkModel');

NAMESPACE('chlk.templates.attendance', function () {

    /** @class chlk.templates.attendance.ClassList*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/ClassListPage.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.ClassList)],
        'ClassList', EXTENDS(chlk.templates.common.PageWithClasses), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.ClassAttendance), 'items',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.AttendanceReason), 'reasons',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'date',

            [ria.templates.ModelPropertyBind],
            Boolean, 'byLastName',

            [ria.templates.ModelPropertyBind],
            String, 'readOnlyReason',

            Boolean, function hasLeftRightToolBar(){
                return true;
            },

            chlk.models.common.ActionLinkModel, function createActionLinkModel_(action, title, isPressed_, args_){
                return new chlk.models.common.ActionLinkModel('attendance', action, title, isPressed_, args_);
            },

            ArrayOf(chlk.models.common.ActionLinkModel), function getLinksDataForLeftSide(){
                return [];
            },

            ArrayOf(Object), function getAdditionalParams(){return [];},

            Object, function getItemClassType(){return chlk.templates.attendance.ClassAttendanceTpl;},

            chlk.models.common.ActionLinkModel, function getDataForDatePicker(){
                var topData = this.getModel().getTopData();
                var selectedId = topData ? topData.getSelectedItemId() : null;
                return this.createActionLinkModel_('classList', null, null
                    , selectedId ? [selectedId.valueOf()] : null)
            },

            ArrayOf(chlk.models.common.ActionLinkModel), function getLinksDataForRightSide(){
                return [
                    new this.createActionLinkModel_('classList', Msg.List, true),
                    new this.createActionLinkModel_('seatingChart', 'Seat chart')
                ];
            },

            ArrayOf(chlk.models.attendance.ClassAttendance), function getPresentStudents(){
                return (this.getItems() || []).filter(function(_){
                    return _.getType() == chlk.models.attendance.AttendanceTypeEnum.PRESENT.valueOf();
                });
            },
            Boolean, function canPost(){
                return this.getModel().canPost();
            }
        ]);
});