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
                return this.format(format_ || 'mm-dd-yy');
            },

            [[String]],
            String, function format(format){
                return $.datepicker.formatDate(format, this.getDate() || getDate());
            },

            VOID, function deserialize(raw) {
                var date = raw ? getDate(raw) : getDate();
                this.setDate(date);
            }
        ]);
});
