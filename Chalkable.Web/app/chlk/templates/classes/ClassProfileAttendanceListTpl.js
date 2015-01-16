REQUIRE('chlk.templates.attendance.ClassList');
REQUIRE('chlk.models.attendance.ClassList');
REQUIRE('chlk.templates.classes.ClassProfileAttendanceListItemTpl');

NAMESPACE('chlk.templates.classes', function(){
   "use strict";

    /**@class chlk.templates.classes.ClassProfileAttendanceListTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/attendance/ClassListPage.jade')],
        [ria.templates.ModelBind(chlk.models.attendance.ClassList)],
        'ClassProfileAttendanceListTpl', EXTENDS(chlk.templates.attendance.ClassList),[

            OVERRIDE, Boolean, function hasLeftRightToolBar(){return false;},

            OVERRIDE, ArrayOf(chlk.models.common.ActionLinkModel), function getLinksDataForLeftSide(){
                var classId = this.getModel().getTopData().getSelectedItemId();
                return [
                    new chlk.models.common.ActionLinkModel('class', 'attendance', 'Back', false, [classId], ['back-btn'])
                ];
            },

            OVERRIDE, chlk.models.common.ActionLinkModel, function getDataForDatePicker(){
                var res = BASE();
                res.getArgs().push(false);
                return res;
            },

            OVERRIDE, Object, function getItemClassType(){
                return chlk.templates.classes.ClassProfileAttendanceListItemTpl;
            },

            OVERRIDE, ArrayOf(chlk.models.common.ActionLinkModel), function getLinksDataForRightSide(){
                var date = this.getModel().getDate();
                var formatDate = date ? date.toStandardFormat() : null;
                return [
                    new this.createActionLinkModel_('classList', Msg.List, true, [formatDate, true]),
                    new this.createActionLinkModel_('classList', Msg.Card, false, [formatDate, true])
                ];
            }
    ]);
});