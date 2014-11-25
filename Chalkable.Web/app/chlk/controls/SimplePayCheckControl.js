REQUIRE('chlk.controls.BaseCheckControl');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.SimplePayCheckControl*/
    CLASS(
        'SimplePayCheckControl', EXTENDS(chlk.controls.BaseCheckControl), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/paycheck.jade')(this);
                ASSET('~/assets/jade/controls/short-pay-check.jade')(this);
            },

            function $(){
                BASE();
                this.classId_ = null;
                this.appId_ = null;
            },

            [[Object]],
            OVERRIDE, Object, function processAttrs(attributes) {
                this.classId_ = attributes.classId;
                this.appId_ = attributes.appId;
                attributes = BASE(attributes);
                return attributes;
            },

            OVERRIDE, Object, function getInstallData(){
                var res = BASE();
                res.appId = this.appId_ && chlk.models.id.AppId(this.appId_);
                res.classes = this.classId_ && [chlk.models.id.AppInstallGroupId(this.classId_)];
                return res;
            }
        ])
});