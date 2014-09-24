REQUIRE('chlk.templates.profile.SchoolPersonProfileTpl');
REQUIRE('chlk.converters.attendance.AttendanceTypeToNameConverter');
REQUIRE('chlk.models.student.StudentProfileSummaryViewData');

NAMESPACE('chlk.templates.student', function () {

    /** @class chlk.templates.student.StudentProfileSummaryTpl*/
    //todo rename this class to StudentProfileSummaryTpl
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/student/SummaryView.jade')],
        [ria.templates.ModelBind(chlk.models.student.StudentProfileSummaryViewData)],
        'StudentProfileSummaryTpl', EXTENDS(chlk.templates.profile.SchoolPersonProfileTpl.OF(chlk.models.student.StudentSummary)), [

            function $(){
                BASE();
                this._converter = new chlk.converters.attendance.AttendanceTypeToNameConverter();
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

            String, function getAvatarTitle(){
                return (getDate().getFullYear() + 12 - this.getUser().getGradeLevelNumber()).toString();
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

            Object, function buildRankGlanceBoxData(){
                return this.buildGlanceBoxData_(this.getUser().getRankBox()
                    , function(item){ return item.getRank; }
                    , function(item){ return  item.getMarkingPeriodName; }
                    , Msg.Percentile, true);
            },

            Object, function buildAttendanceGlanceBoxData(){
                return this.buildGlanceBoxData_(this.getUser().getAttendanceBox()
                    , function(item){ return item.getAbsences}
                    , function(item){ return item.getClazz().getName}
                    , Msg.Attendance);
            },

            Object, function buildDisciplineGlanceBoxData(){
                return this.buildGlanceBoxData_(this.getUser().getDisciplineBox()
                    , function(item){ return item.getTotal}
                    , function(item){ return item.getDisciplineType().getName}
                    , Msg.Discipline);
            },

            Object, function buildGradesGlanceBoxData(){
                return this.buildGlanceBoxData_(this.getUser().getGradesBox()
                    , function(item){ return item.getGrade}
                    , function(item){ return item.getAnnouncementTitle}
                    , Msg.Recent, null, 'announcement', 'view'
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
                            getTotalMethod: getTotalMethod(hoverItems[i]),
                            getSummaryMethod: getSummaryMethod(hoverItems[i]),
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
        ]);
});