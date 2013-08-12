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

NAMESPACE('chlk.models.common', function () {
    "use strict";
    /** @class chlk.models.common.ChlkDate*/
    CLASS(
        'ChlkDate', IMPLEMENTS(ria.serialize.IDeserializable), [

            Date, 'date',

            [[String]],
            String, function toString(format_){
                return this.format(format_ || 'm-dd-yy');
            },

            [[String]],
            String, function format(format){
                format = format.replace(/min/g, this.timepartToStr(this.getDate().getMinutes()));
                var res =$.datepicker.formatDate(format, this.getDate() || getDate());
                res = res.replace(/hh/g, this.timepartToStr(this.getDate().getHours()));
                res = res.replace(/ss/g, this.timepartToStr(this.getDate().getSeconds()));
                return res;
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
