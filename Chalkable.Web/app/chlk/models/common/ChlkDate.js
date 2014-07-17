REQUIRE('ria.serialize.IDeserializable');

window.currentDate = new Date();

function getDate(str,a,b){
    if(str){
        str = str.replace ? str.replace(/(-|\.)/g, '/') : str;
        return ( a !== undefined && b !== undefined ? new Date(str,a,b) : new Date(str));
    }
    var serverTime = new Date(window.serverTime.replace(/(-|\.)/g, "/"));
    var now = new Date();
    if(serverTime.getDate() == now.getDate() && serverTime.getMonth() == now.getMonth() && serverTime.getFullYear() == now.getFullYear())
        return new Date();
    var dt = new Date(serverTime.getTime() + now.getTime() - window.currentDate.getTime());
    dt.setSeconds(now.getSeconds());
    dt.setMinutes(now.getMinutes());
    dt.setHours(now.getHours());
    return dt;
}

function formatDate(date, format){
    return $.datepicker.formatDate(format, date || getDate());
}

function getDateDiffInDays(begin, end){
    var b = new Date(begin.getFullYear(), begin.getMonth(), begin.getDate());
    var e = new Date(end.getFullYear(), end.getMonth(), end.getDate());
    return Math.ceil((e - b)/1000/3600/24);
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
                this._STANDART_FORMAT = 'mm-dd-yy'
                this._DEFAULT_FORMAT = 'm-dd-yy';
                this._USA_DATE_FORMAT = 'm/dd/yy';
                this._USA_DATE_TIME_FORMAT = 'm/dd/yy hh:min:ss tt'
            },

            [[String]],
            String, function toString(format_){
                return this.format(format_ || this._DEFAULT_FORMAT);
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
                    case dateEnum.YEAR: res = getDate(y + count, mon, day, h, min, sec); break;
                    case dateEnum.MONTH: res = getDate(y, mon + count, day, h, min, sec); break;
                    default: res = thisDate.getTime() + count * chlk.models.common.MillisecondsEnum[type.valueOf()].valueOf();
                }
                date.setDate(getDate(res));
                return date;
            },

            [[String]],
            String, function format(format){
                format = format.replace(/min/g, this.timepartToStr(this.getDate().getMinutes()));
                var res =$.datepicker.formatDate(format, this.getDate() || getDate());
                var hours = this.getDate().getHours();
                res = res.replace(/hh/g, this.timepartToStr(hours));
                res = res.replace(/ss/g, this.timepartToStr(this.getDate().getSeconds()));
                res = res.replace(/tt/g, hours > 11 ? 'pm' : 'am');
                return res;
            },

            String, function toStandardFormat(){
                return this.format(this._STANDART_FORMAT);
            },

            String, function toUSADateTimeFormat(){
                return this.format(this._USA_DATE_TIME_FORMAT);
            },

            [[SELF]],
            Boolean, function isSameDay(date){
                //VALIDATE_ARG('date', [SELF], date);
                return this.toStandardFormat() == date.toStandardFormat();
            },

            VOID, function deserialize(raw) {
                var date = raw ? getDate(raw) : getDate();
                this.setDate(date);
            },

            [[Number]],
            String, function timepartToStr(t) {
                return "" + ((t - t % 10) / 10) + (t % 10);
            },

            Number, function getDaysInMonth(month_, year_) {
                var month = this.getDate().getMonth() || month_;
                var year = this.getDate().getFullYear() || year_;
                return getDate(year, month + 1, 0).getDate();
            },

            [[SELF, SELF]],
            function getDateDiffInDays(d1, d2) {
                var a = d1.getDate();
                var b = d2.getDate();
                var _MS_PER_DAY = 1000 * 60 * 60 * 24;
                // Discard the time and time-zone information.
                var utc1 = Date.UTC(a.getFullYear(), a.getMonth(), a.getDate());
                var utc2 = Date.UTC(b.getFullYear(), b.getMonth(), b.getDate());
                return Math.floor((utc2 - utc1) / _MS_PER_DAY);
            },

            [[Date]],
            SELF, function getLastDay(date_){
                var date = date_ || this.getDate();
                var y = date.getFullYear();
                var m = date.getMonth();
                var lastDay = getDate(y, m + 1, 0);
                return new chlk.models.common.ChlkDate(lastDay);
            }

        ]);
});
