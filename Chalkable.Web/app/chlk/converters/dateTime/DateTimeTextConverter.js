REQUIRE('ria.templates.IConverter');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.converters.dateTime', function () {

    /** @class chlk.converters.dateTime.DateTimeTextConverter */
    CLASS(
        'DateTimeTextConverter', IMPLEMENTS(ria.templates.IConverter), [
            [[Object]],
            String, function convert(time) {
                VALIDATE_ARG('time', chlk.models.common.ChlkDate, time);
                if (!time)
                    return "";
                var nowDate = new Date();
                var totalMins = Math.ceil((nowDate - time.getDate()) / 1000 / 60);
                var minutes = totalMins % 60;
                var totalHours = (totalMins - minutes) / 60;
                var hours = totalHours % 24; //Math.floor(dif / 60);
                var totalDays = (totalHours - hours) / 24;
                var days = totalDays % 7;
                var weeks = (totalDays - days) / 7;
                var res;
                if(weeks){
                    if(weeks == 1){
                        res = 'one weeks ago';
                    }else{
                        res = "more than one week ago";
                    }
                }else{
                    if(days){
                        if(days == 1){
                            res = 'yesterday';
                        }else{
                            res = days + ' days ago';
                        }
                    }else{
                        if(hours){
                            res = hours + ' hours ';
                            if(minutes){
                                res = res + 'and ' + minutes + ' minutes '
                            }
                            res = res + 'ago';
                        }else{
                            res = minutes + ' minutes ago';
                        }
                    }
                }
                return res;
            }
        ])
});
