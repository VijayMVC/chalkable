REQUIRE('chlk.templates.common.PageWithGrades');
REQUIRE('chlk.models.admin.Home');

NAMESPACE('chlk.templates.admin', function () {

    /** @class chlk.templates.admin.AdminHomeTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/admin/HomePage.jade')],
        [ria.templates.ModelBind(chlk.models.admin.Home)],
        'AdminHomeTpl', EXTENDS(chlk.templates.common.PageWithGrades), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.attendance.StudentAttendances), 'attendances',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.discipline.StudentDisciplines), 'disciplines',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.grading.GradingStat), 'gradingStats',

            [ria.templates.ModelPropertyBind],
            chlk.models.attendance.AttendanceStatBox, 'notInClassBox',

            [ria.templates.ModelPropertyBind],
            chlk.models.attendance.AttendanceStatBox, 'absentForDay',

            [ria.templates.ModelPropertyBind],
            chlk.models.attendance.AttendanceStatBox, 'absentForMp',

            [ria.templates.ModelPropertyBind],
            chlk.models.funds.BudgetBalance, 'budgetBalance',

            [ria.templates.ModelPropertyBind],
            String, 'markingPeriodName',

            [ria.templates.ModelPropertyBind],
            Boolean, 'forGradeLevels',

            [[chlk.models.attendance.AttendanceStatBox]],
            function getChartData(info){
                var categories = [], data = [];
                info.getStat() && info.getStat().forEach(function(item){
                    categories.push(item.getSummary());
                    data.push(item.getStudentCount());
                });
                var max0 = 20;
                var maxLength = 30;
                var dataLen = data.length;
                if(dataLen > maxLength){
                    var resData = [], resCategories = [];
                    for(var i = 0; i < maxLength; i++){
                        resData.push(data[parseInt(dataLen*i/maxLength, 10)]);
                        resCategories.push(categories[parseInt(dataLen*i/maxLength, 10)]);
                    }
                    resData.push(data[dataLen-1]);
                    resCategories.push(categories[dataLen-1]);
                    data = resData;
                    categories = resCategories;
                }

                data.forEach(function(item, i){
                    if(item > max0)
                        max0 = item;
                });
                var max = Math.ceil(max0 / 10) * 10;
                return {
                    categories : categories,
                    data : data,
                    max : max
                }
            },

            function getGradingChartData(){
                var gradingStats = this.getGradingStats();
                var names = [], values = [], allValues = [];
                gradingStats.forEach(function(item){
                    var name = item.getTitle().length > 10 ? item.getTitle().slice(0,8) + '...' : item.getTitle();
                    names.push(name);
                    values.push(item.getAvgByGradeLevels() || 0);
                    allValues.push(item.getFullAvg() || 0);
                });

                var series = [{
                    name: '',
                    data: allValues
                }];

                if(this.isForGradeLevels()){
                    series.unshift({
                        name: '',
                        data: values
                    })
                }
                return {
                    names : names,
                    series : series
                }
            },

            [[chlk.models.funds.BudgetBalance]],
            function getBudgetChartData(info){
                var percentSpent = (info.getPercentSpent() > 100) ? 100 : info.getPercentSpent();

                 var data = [{
                            y: 100 - percentSpent,
                            color: {
                                radialGradient: { cx: 0.5, cy: 0.3, r: 0.7 },
                                stops: [
                                    [0, '#296882'],
                                    [1, '#2e738d']
                                ]
                            },
                            borderColor: '#134252',
                            drilldown: {
                                name: 'MSIE versions',
                                categories: [''],
                                data: [100 - percentSpent],
                                color: '#30708a',
                                border: 'black'
                            }
                        }, {
                            y: percentSpent,
                            color: 'transparent',
                            borderWidth: 0,
                            drilldown: {
                                name: 'MSIE versions',
                                categories: [''],
                                data: [percentSpent],
                                color: 'transparent'
                            }
                        }];

                var versionsData = [];
                for (var i = 0; i < data.length; i++) {

                    // add version data
                    for (var j = 0; j < data[i].drilldown.data.length; j++) {
                        versionsData.push({
                            name: data[i].drilldown.categories[j],
                            y: data[i].drilldown.data[j],
                            color: data[i].color,
                            borderColor: data[i].borderColor,
                            borderWidth: data[i].borderWidth
                        });
                    }
                }
                return versionsData;
            }
        ])
});