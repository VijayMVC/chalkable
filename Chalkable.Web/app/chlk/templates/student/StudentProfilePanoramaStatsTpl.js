REQUIRE('chlk.templates.ChlkTemplate');
REQUIRE('chlk.models.student.StudentPanoramaViewData');

NAMESPACE('chlk.templates.student', function () {

    /** @class chlk.templates.student.StudentProfilePanoramaStatsTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/student/StudentProfilePanoramaStats.jade')],
        [ria.templates.ModelBind(chlk.models.student.StudentPanoramaViewData)],
        'StudentProfilePanoramaStatsTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            function getTestsChartOptions_(){
                var standardizedTestsStats = this.getModel().getPanoramaInfo().getStandardizedTestsStats() || [],
                    categories = [], series = [];

                if(!standardizedTestsStats.length)
                    return null;

                standardizedTestsStats.forEach(function(item, index){
                    var data = item.getDailyStats(), columnData = [], color;

                    switch(index){
                        case 0: color = "#f28a02"; break;
                        case 1: color = "#2d7de7"; break;
                        case 2: color = "#3fc47b"; break;
                        case 3: color = "#97c53c"; break;
                    }

                    data.forEach(function(stat){
                        columnData.push(stat.getNumber());
                        categories.push(stat.getSummary());
                    });

                    series.push({
                        type: 'line',
                        name: item.getStandardizedTest().getDisplayName() + ' | ' + item.getComponent().getName() + ' | ' + item.getScoreType().getName(),
                        data: columnData,
                        color: color
                    })

                });

                return {
                    chart:{
                        height: 200
                    },

                    plotOptions:{
                        line:{
                            marker: {
                                fillColor: '#ffffff',
                                symbol: 'circle',
                                radius: 5,
                                lineWidth: 3,
                                enabled: true,
                                lineColor: null
                            }
                        }
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
                        gridLineDashStyle: 'solid',
                        startOnTick: true,
                        showFirstLabel: true//,
                        //min: 0
                    },

                    series: series
                }
            },

            [[Date]],
            function getDaysBeforeCount(date){
                var daysBeforeInCurMonth = date.format('d') - 1,
                    dayOfAWeek = date.format('w'),
                    daysBeforeInPrevMonth = (dayOfAWeek - daysBeforeInCurMonth) % 7;
                
                if(daysBeforeInPrevMonth < 0)
                    daysBeforeInPrevMonth += 7;
                
                return daysBeforeInPrevMonth;
            },

            function getDayItemInfo(dayItem, dayIndex){
                var classes = [], disciplinesTooltip, tooltip;
                if(dayItem){
                    var dateFormat = dayItem.date && dayItem.date.format('m.d.Y ');
                    if(dayIndex % 7 == 0)
                        classes.push('sunday');
                    if(dayIndex % 7 == 6)
                        classes.push('saturday');
                    if(dayItem.active)
                        classes.push('active-item');
                    if(dayItem.isAbsent){
                        classes.push('absent');
                        tooltip = dateFormat + "Absent full day";
                    }
                    if(dayItem.isHalfAbsent){
                        classes.push('half-absent');
                        tooltip = dateFormat + "Absent 1/2 day";
                    }
                    if(dayItem.isLate){
                        classes.push('late');
                        tooltip = dateFormat + "Tardy";
                    }
                    if(dayItem.isFuture)
                        classes.push('future');
                    if(dayItem.disciplines && dayItem.disciplines.length){
                        classes.push('has-disciplines');
                        disciplinesTooltip = dayItem.disciplines.map(function(item){
                            return dateFormat + item;
                        }).join('\n');
                        tooltip = tooltip ? tooltip + '\n' + disciplinesTooltip : disciplinesTooltip;
                    }
                }
                return {
                    classes : classes.join(' '),
                    tooltip : tooltip
                };
            },

            function getDayItemClass(dayItem, dayIndex){
                var res = [];
                if(dayItem){
                    if(dayIndex % 7 == 0)
                        res.push('sunday');
                    if(dayIndex % 7 == 6)
                        res.push('saturday');
                    if(dayItem.active)
                        res.push('active-item');
                    if(dayItem.isAbsent)
                        res.push('absent');
                    if(dayItem.isHalfAbsent)
                        res.push('half-absent');
                    if(dayItem.isLate)
                        res.push('late');
                    if(dayItem.isFuture)
                        res.push('future');
                    if(dayItem.disciplines && dayItem.disciplines.length)
                        res.push('has-disciplines');
                }
                return res.join(' ');
            },

            function getDayItemDisciplines(dayItem){
                var res = '';
                if(dayItem.disciplines && dayItem.disciplines.length){
                    var dateFormat = dayItem.date.format('m.d.Y ');
                    res = dayItem.disciplines.map(function(item){
                        return dateFormat + item;
                    }).join('\n');
                }
                return res;
            },

            function preparePanoramaCalendars(){
                var that = this, today = getDate();
                this.getModel().getPanoramaInfo().getCalendars().forEach(function(schoolYear){
                    var month, res = [], curDt, curMonth, daysBeforeInPrevMonth,
                        days, daysBeforeInCurMonth, monthTitle, isFuture = false;
                    schoolYear.getCalendarItems().forEach(function(item, index, arr){
                        curDt = item.getDate();
                        curMonth = curDt.format('n');
                        var curDay = parseInt(curDt.format('d'), 10);

                        if(curDt > today)
                            isFuture = true;

                        if(month != curMonth){
                            daysBeforeInCurMonth = curDay - 1;
                            daysBeforeInPrevMonth = that.getDaysBeforeCount(curDt);
                            if(month)
                                res.push({
                                    days: [].slice.call(days),
                                    title: monthTitle
                                });
                            days = [];
                            for(var i = 0; i < daysBeforeInPrevMonth; i++)
                                days.push(null);
                           // if(month)
                            for(var j = 0; j < daysBeforeInCurMonth; j++)
                                days.push({
                                    active: false,
                                    isFuture: isFuture,
                                    day: curDay - daysBeforeInCurMonth + j
                                });
                            month = curMonth;
                            monthTitle = curDt.format('M Y');
                        }

                        days.push({
                            active: true,
                            isAbsent: item.isAbsent(),
                            isFuture: isFuture,
                            date: curDt,
                            absenceLevel: item.getAbsenceLevel(),
                            isHalfAbsent: item.isHalfAbsent(),
                            isLate: item.isLate(),
                            disciplines: item.getDisciplines(),
                            day: curDay
                        });

                        if(arr[index + 1]){
                            var nextDt = arr[index + 1].getDate(),
                                daysDiff = Math.ceil((nextDt - curDt)/1000/60/60/24);
                            if(daysDiff > 1){
                                if(nextDt.format('n') != curDt.format('n'))
                                    daysDiff = parseInt(curDt.format('t'), 10) - curDay + 1;
                                for(var k = 0; k < daysDiff - 1; k++)
                                    days.push({
                                        active: false,
                                        isFuture: isFuture,
                                        day: curDay + k + 1
                                    });

                            }
                        }
                    });

                    if(monthTitle){
                        res.push({
                            days: [].slice.call(days),
                            title: monthTitle
                        });
                    }

                    schoolYear.setItemsByMonth(res);
                })
            }
        ]);
});