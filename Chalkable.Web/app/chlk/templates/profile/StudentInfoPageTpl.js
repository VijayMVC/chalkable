REQUIRE('chlk.templates.profile.SchoolPersonInfoPageTpl');
REQUIRE('chlk.models.student.StudentProfileInfoViewData');
REQUIRE('chlk.templates.student.StudentProfilePanoramaStatsTpl');
REQUIRE('chlk.templates.student.StudentProfilePanoramaDisciplineStatsTpl');
REQUIRE('chlk.templates.student.StudentProfilePanoramaAttendanceStatsTpl');

NAMESPACE('chlk.templates.profile', function () {
    "use strict";
    /** @class chlk.templates.profile.StudentInfoPageTpl*/

    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/profile/student-info-page.jade')],
        [ria.templates.ModelBind(chlk.models.student.StudentProfileInfoViewData)],
        [ria.mvc.PartialUpdateRule(chlk.templates.student.StudentProfilePanoramaStatsTpl, '', '.panorama-stats', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.student.StudentProfilePanoramaAttendanceStatsTpl, 'sort-attendances', '.attendance-grid', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.student.StudentProfilePanoramaDisciplineStatsTpl, 'sort-disciplines', '.discipline-grid', ria.mvc.PartialUpdateRuleActions.Replace)],
        'StudentInfoPageTpl', EXTENDS(chlk.templates.profile.SchoolPersonProfileTpl.OF(chlk.models.student.StudentInfo)), [

            OVERRIDE, String, function render() {
                var res = BASE();
                var user = this.getModel().getUser();
                user.setAbleEdit(user.isAbleEdit()
//                    && this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_ADDRESS)
//                    && this.hasUserPermission_(chlk.models.people.UserPermissionEnum.MAINTAIN_PERSON)
                );
                return res;
            },

            String, function getCurrentAttendanceLevel(){
                return this.getUser().getCurrentAttendanceLevel();
            },

            Number, function getCurrentAttendanceType(){
                return this.getUser().getCurrentAttendanceType();
            },

            String, function getAttendanceTypeName(){
                return this._converter.convert(this.getCurrentAttendanceType());
            },

            Object, function getStatusData(){
                var attType =this.getCurrentAttendanceType();
                var res = {};
                if(attType == chlk.models.attendance.AttendanceTypeEnum.NA.valueOf()
                    || !this.getUser().getCurrentClassName() || !this.getUser().getRoomId()){
                    res.statusName = 'No attendance taken';
                    res.status = 'not-assigned';
                }else{
                    res.statusName = this.getAttendanceTypeName();
                    res.status = res.statusName && res.statusName.toLowerCase();
                }
                return res;
            },

            Object, function buildAttendanceGlanceBoxData(){
                return this.buildGlanceBoxData_(this.getUser().getAttendanceBox()
                    , function(item){
                        return item.getAbsences()
                    }
                    , function(item){
                        return item.getClazz().getName()
                    }
                    , Msg.Attendance);
            },

            Object, function buildDisciplineGlanceBoxData(){
                return this.buildGlanceBoxData_(this.getUser().getDisciplineBox()
                    , function(item){ return item.getTotal()}
                    , function(item){ return item.getDisciplineType().getName()}
                    , Msg.Discipline.toString());
            },

            Object, function buildGradesGlanceBoxData(){
                return this.buildGlanceBoxData_(this.getUser().getGradesBox()
                    , function(item){ return item.getGrade()}
                    , function(item){ return item.getAnnouncementTitle()}
                    , Msg.Recent, null, this.getUserRole().isAdmin() ? null : 'announcement', this.getUserRole().isAdmin() ? null : 'view'
                    , function(item){ return [item.getAnnouncementId()]});
            },

            [[Object, Function, Function, String, Boolean, String, String, Function]],
            Object, function buildGlanceBoxData_(boxData, getTotalMethod, getSummaryMethod, title, noHover_, controller_, action_, paramsMethod_){
                var items = [];
                var hoverItems = boxData.getHover();
                if(hoverItems)
                    for(var i = 0; i < hoverItems.length; i++){
                        items.push({
                            data: hoverItems[i],
                            total: getTotalMethod(hoverItems[i]),
                            summary: getSummaryMethod(hoverItems[i]),
                            controller: controller_,
                            action: action_,
                            params: paramsMethod_ ? paramsMethod_(hoverItems[i]) : []
                        });
                    }
                return {
                    value: boxData.getTitle(),
                    items: noHover_ ? null : items,
                    title: title
                };
            }
        ])
});
