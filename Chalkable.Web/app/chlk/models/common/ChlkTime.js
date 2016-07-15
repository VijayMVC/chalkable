REQUIRE('ria.serialize.IDeserializable');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    var APM = ['am', 'pm'];

    /** @class chlk.models.common.ChlkTime*/
    CLASS(
        'ChlkTime', IMPLEMENTS(ria.serialize.IDeserializable), [

            function $(){
                BASE();
                this._DEFAULT_TIME_SEPARATOR = ':';
            },
            Number, 'hours',
            Number, 'minutes',

            [[String]],
            String, function toString(){
                return this.format(true);
            },

            [[Boolean, String]],
            String, function format(isRegularTime_, timeSeparator_){
                var h = this.getHours();
                var min = this.getMinutes();
                if(min < 10) min = '0' + min;
                var apm = '';
                timeSeparator_ = timeSeparator_ || this._DEFAULT_TIME_SEPARATOR;
                if(!isRegularTime_){
                    apm = APM[Math.floor(h / 12)];
                    h %= 12;
                    if(h < 1) h = 12;
                }
                return h + timeSeparator_ + min + ' ' + apm;
            },

            VOID, function deserialize(raw) {
                var h = Math.floor(raw / 60);
                this.setHours(h % 24);
                this.setMinutes(raw % 60);
            }
        ]);
});
