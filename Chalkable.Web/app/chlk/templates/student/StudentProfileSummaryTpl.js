REQUIRE('chlk.templates.profile.SchoolPersonProfileTpl');
REQUIRE('chlk.converters.attendance.AttendanceTypeToNameConverter');
REQUIRE('chlk.models.student.StudentProfileSummaryViewData');

NAMESPACE('chlk.templates.student', function () {

    /** @class chlk.templates.student.StudentProfileSummaryTpl*/
    //todo rename this class to StudentProfileSummaryTpl
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/student/SummaryView.jade')],
        [ria.templates.ModelBind(chlk.models.student.StudentProfileSummaryViewData)],
        'StudentProfileSummaryTpl', EXTENDS(chlk.templates.profile.SchoolPersonProfileTpl), [

            function $(){
                BASE();
                this._converter = new chlk.converters.attendance.AttendanceTypeToNameConverter();
            },

            Number, function getCurrentAttendanceType(){return this.getUser().getCurrentAttendanceType();},
            String, function getAttendanceTypeName(){
                return this._converter.convert(this.getCurrentAttendanceType());
            },

            String, function getAvatarTitle(){
                return (getDate().getFullYear() + 12 - this.getUser().getGradeLevelNumber()).toString();
            },

            Object, function getStatusData(){
                var attType =this.getCurrentAttendanceType();
                var res = {};
                if(attType == chlk.models.attendance.AttendanceTypeEnum.NA){
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
                    , function(item){console.log(item.getMarkingPeriodName()); return item.getRank; }
                    , function(item){console.log(item.getRank()); return  item.getMarkingPeriodName; }
                    , Msg.Recent);
            },

            Object, function buildAttendanceGlanceBoxData(){
                return this.buildGlanceBoxData_(this.getUser().getAttendanceBox()
                    , function(item){ return item.getAttendanceCount}
                    , function(item){ return item.getType}
                    , Msg.Attendance);
            },

            Object, function buildDisciplineGlanceBoxData(){
                return this.buildGlanceBoxData_(this.getUser().getDisciplineBox()
                    , function(item){ return item.getCount}
                    , function(item){ return item.getDisciplineName}
                    , Msg.Discipline);
            },

            Object, function buildGradesGlanceBoxData(){
                return this.buildGlanceBoxData_(this.getUser().getGradesBox()
                    , function(item){ return item.getGrade}
                    , function(item){ return item.getAnnouncementTypeName}
                    , Msg.Percentile);
            },

            [[Object, Function, Function, String]],
            Object, function buildGlanceBoxData_(boxData, getTotalMethod, getSummaryMethod, title){
                var items = [];
                var hoverItems = boxData.getHover();
                for(var i = 0; i < hoverItems.length; i++){
                    items.push({
                        data: hoverItems[i],
                        getTotalMethod: getTotalMethod(hoverItems[i]),
                        getSummaryMethod: getSummaryMethod(hoverItems[i])
                    });
                }
                return {
                    value: boxData.getTitle(),
                    items: items,
                    title: title
                };
            }
        ]);
});