REQUIRE('ria.serialize.IDeserializable');

function getDate(str,a,b){
    if(typeof str == "string" && str.length == 10 ){
        return new Date(str.replace(/-/g, '/'));
    }
    else{
        return str ? ( a !== undefined && b !== undefined ? new Date(str,a,b) : new Date(str))  : (new Date());
    }
}

function formatDate(date, format){
    return $.datepicker.formatDate(format, date || getDate());
}


function getDateDiffInDays(begin, end){
    return Math.ceil((end - begin)/1000/3600/24);
}

NAMESPACE('chlk.models.common', function () {
    "use strict";

    var second = 1000,
        minute = 60 * second,
        hour = 60 * minute,
        day = 24 * hour,
        week = 7 * day;

    /** @class chlk.models.common.MillisecondsEnum*/
    ENUM('MillisecondsEnum', {
        SECOND: 1000,
        MINUTE: 60000,
        HOUR: 3600000,
        DAY: 86400000,
        WEEK: 604800000
    });

    /** @class chlk.models.common.ChlkDateEnum*/
    ENUM('ChlkDateEnum', {
        SECOND: 'SECOND',
        MINUTE: 'MINUTE',
        HOUR: 'HOUR',
        DAY: 'DAY',
        WEEK: 'WEEK',
        MONTH: 'MONTH',
        YEAR: 'YEAR'
    });

    /** @class chlk.models.common.ChlkDate*/
    CLASS(
        'ChlkDate', IMPLEMENTS(ria.serialize.IDeserializable), [

            Date, 'date',

            [[Date]],
            function $(date_){
                BASE();
                date_ && this.setDate(date_);
            },

            [[String]],
            String, function toString(format_){
                return this.format(format_ || 'm-dd-yy');
            },

            [[chlk.models.common.ChlkDateEnum, Number]],
            SELF, function add(type, count){
                var dateEnum = chlk.models.common.ChlkDateEnum;
                var thisDate = this.getDate();
                var sec = thisDate.getSeconds();
                var min = thisDate.getMinutes();
                var h = thisDate.getHours();
                var day = thisDate.getDate();
                var mon = thisDate.getMinutes();
                var y = thisDate.getFullYear();
                var date = new chlk.models.common.ChlkDate(), res;
                switch (type){
                    case dateEnum.YEAR: res = new Date(y + count, mon, day, h, min, sec); break;
                    case dateEnum.MONTH: res = new Date(y, mon + count, day, h, min, sec); break;
                    default: res = thisDate.getTime() + count * chlk.models.common.MillisecondsEnum[type.valueOf()].valueOf();
                }
                date.setDate(new Date(res));
                return date;
            },

            [[String]],
            String, function format(format){
                format = format.replace(/min/g, this.timepartToStr(this.getDate().getMinutes()));
                var res =$.datepicker.formatDate(format, this.getDate() || getDate());
                res = res.replace(/hh/g, this.timepartToStr(this.getDate().getHours()));
                res = res.replace(/ss/g, this.timepartToStr(this.getDate().getSeconds()));
                return res;
            },

            [[SELF]],
            Boolean, function isSameDay(date){
                //VALIDATE_ARG('date', [SELF], date);
                return this.format('mm-dd-yy') == date.format('mm-dd-yy');
            },

            VOID, function deserialize(raw) {
                var date = raw ? getDate(raw) : getDate();
                this.setDate(date);
            },

            [[Number]],
            String, function timepartToStr(t) {
                return "" + ((t - t % 10) / 10) + (t % 10);
            }

        ]);
});
