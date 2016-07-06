REQUIRE('chlk.templates.profile.SchoolPersonProfileTpl');
REQUIRE('chlk.converters.attendance.AttendanceTypeToNameConverter');
REQUIRE('chlk.models.student.StudentProfilePanoramaViewData');

NAMESPACE('chlk.templates.student', function () {

    /** @class chlk.templates.student.StudentProfilePanoramaTpl*/
    //todo rename this class to StudentProfileSummaryTpl
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/student/StudentProfilePanorama.jade')],
        [ria.templates.ModelBind(chlk.models.student.StudentProfilePanoramaViewData)],
        'StudentProfilePanoramaTpl', EXTENDS(chlk.templates.profile.SchoolPersonProfileTpl.OF(chlk.models.student.StudentPanoramaViewData)), [

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
            },

            function getTestsChartOptions_(){
                var standardizedTestsStats = this.getUser().getPanoramaInfo().getStandardizedTestsStats() || [],
                    categories = [], series = [];

                if(!standardizedTestsStats.length)
                    return null;

                standardizedTestsStats.forEach(function(item, index){
                    var data = item.getDailyStats(), columnData = [];

                    data.forEach(function(stat){
                        columnData.push(stat.getNumber());
                        categories.push(stat.getSummary());
                    });

                    series.push({
                        type: 'line',
                        name: item.getStandardizedTest().getDisplayName() + ' | ' + item.getComponent().getName() + ' | ' + item.getScoreType().getName(),
                        data: columnData
                    })

                });

                return {
                    chart:{
                        height: 200
                    },

                    legend:{
                        enabled: true
                    },

                    xAxis: {
                        categories: categories,
                        gridLineWidth:0,
                        lineWidth:0
                    },

                    yAxis: {
                        gridLineWidth: 1,
                        lineWidth: 1,
                        lineColor: '#ebebeb',
                        gridLineColor: '#ebebeb',
                        gridLineDashStyle: 'solid'
                    },

                    series: series
                }
            },

            function getAbsencesChartOptions_(){
                var stats = this.getUser().getPanoramaInfo().getStudentAbsenceStats() || [],
                    columnData = [], absenceLevels = ['Absent', 'Half Day', 'All Day'],
                    categoriesNames = ['A', 'H', 'D'];

                if(!stats.length)
                    return null;

                stats.forEach(function(item, index){
                    columnData.push([item.getDate().getDate().getTime(), absenceLevels.indexOf(item.getAbsenceLevel())]);
                });

                return {
                    chart:{
                        height: 200
                    },

                    xAxis: {
                        type: 'datetime',
                        labels: {
                            formatter: function() {
                                return Highcharts.dateFormat('%b %Y', this.value);
                            }
                        },
                        gridLineWidth:0,
                        lineWidth:0
                    },

                    yAxis: {
                        gridLineWidth: 1,
                        lineWidth: 1,
                        lineColor: '#ebebeb',
                        gridLineColor: '#ebebeb',
                        gridLineDashStyle: 'solid',
                        labels: {
                            formatter: function () {
                                return categoriesNames[this.value];
                            }
                        }
                    },

                    tooltip: {
                        formatter: function () {
                            return '<b>' + absenceLevels[this.y] + '</b>';
                        }
                    },

                    series: [{
                        type: 'area',
                        data: columnData,
                        color: '#41b768'
                    }]
                }
            },

            function getDisciplinesChartOptions_(){
                var stats = this.getUser().getPanoramaInfo().getDailyDisciplines() || [],
                    columnData = [];

                if(!stats.length)
                    return null;

                stats.forEach(function(item, index){
                    columnData.push([item.getDate().getDate().getTime(), item.getNumber()]);
                });

                return {
                    chart:{
                        height: 200
                    },

                    plotOptions:{
                        line: {
                            marker: {
                                symbol: 'triangle-down'
                            }
                        }
                    },

                    xAxis: {
                        type: 'datetime',
                        labels: {
                            formatter: function() {
                                return Highcharts.dateFormat('%b %Y', this.value);
                            }
                        },
                        gridLineWidth:0,
                        lineWidth:0
                    },

                    yAxis: {
                        gridLineWidth: 1,
                        lineWidth: 1,
                        lineColor: '#ebebeb',
                        gridLineColor: '#ebebeb',
                        gridLineDashStyle: 'solid'
                    },

                    series: [{
                        type: 'area',
                        data: columnData,
                        color: '#f8e6a5'
                    }]
                }
            }
        ]);
});