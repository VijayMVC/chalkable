REQUIRE('chlk.activities.lib.ChlkTemplateActivity');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.activities.lib', function () {
    /** @class chlk.activities.lib.TemplatePage*/

    CLASS(
        'TemplatePage', EXTENDS(chlk.activities.lib.ChlkTemplateActivity), [
            function $() {
                BASE();
                this._wrapper = new ria.dom.Dom('#content-wrapper');
                this._body = new ria.dom.Dom('body');
            },


            Date, function getDate(str_, a_, b_){
                return chlk.models.common.ChlkDate.GET_SERVER_DATE(str_, a_, b_);
            },

            Date, function getSchoolYearServerDate(str_, a_, b_){
                return chlk.models.common.ChlkSchoolYearDate.GET_SCHOOL_YEAR_SEVER_DATE(str_, a_, b_);
            },

            OVERRIDE, VOID, function processAnnotations_(ref) {
                BASE(ref);
                if (ref.isAnnotatedWith(chlk.activities.lib.PageClass)){
                    this._pageClass = ref.findAnnotation(chlk.activities.lib.PageClass)[0].clazz;
                }else{
                    this._pageClass = null;
                }

                if (ref.isAnnotatedWith(chlk.activities.lib.BodyClass)){
                    this._bodyClass = ref.findAnnotation(chlk.activities.lib.BodyClass)[0].clazz;
                }else{
                    this._bodyClass = null;
                }
            },

            [[String]],
            OVERRIDE, VOID, function onModelWait_(msg_) {
                BASE(msg_);

                this._pageClass && this._wrapper.addClass(this._pageClass);
                this._bodyClass && this._body.addClass(this._bodyClass);
            },

            OVERRIDE, VOID, function onStop_() {
                BASE();
                this._pageClass && this._wrapper.removeClass(this._pageClass);
                this._bodyClass && this._body.removeClass(this._bodyClass);
            },

            OVERRIDE, ria.dom.Dom, function onDomCreate_() {
                return new ria.dom.Dom().fromHTML('<div></div>');
            },
        ]);
});
