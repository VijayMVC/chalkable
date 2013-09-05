REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.admin.Home');

NAMESPACE('chlk.templates.admin', function () {

    /** @class chlk.templates.admin.HomePage*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/admin/HomePage.jade')],
        [ria.templates.ModelBind(chlk.models.admin.Home)],
        'HomePage', EXTENDS(chlk.templates.JadeTemplate), [
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

            [[chlk.models.attendance.AttendanceStatBox]],
            function getChartData(info){
                var categories = [], data = [];
                info.getStat() && info.getStat().forEach(function(item){
                    categories.push(item.getSummary());
                    data.push(item.getStudentCount());
                });
                var max0 = data[0] || 20;
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
                var len = categories.length;
                if(len > 8){
                    var res = [], index;
                    categories.forEach(function(item, index){
                        res.push('');
                    });
                    res[0] = categories[0];
                    res[len-1] = categories[len-1];
                    for(var i = 1; i<7; i++){
                        index = parseInt(len*i/7, 10);
                        res[index] = categories[index];
                    }
                    categories = res;
                }
                return {
                    categories : categories,
                    data : data,
                    max : max
                }
            }
        ])
});