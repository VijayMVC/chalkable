REQUIRE('chlk.activities.lib.ChlkTemplateActivity');
REQUIRE('chlk.models.common.ChlkDate');

NAMESPACE('chlk.activities.lib', function () {

    var UNDER_OVERLAY_CLASS = 'under-overlay';

    /** @class chlk.activities.lib.TemplatePage*/
    CLASS(
        'TemplatePage', EXTENDS(chlk.activities.lib.ChlkTemplateActivity), [
            function $() {
                BASE();
                this._wrapper = new ria.dom.Dom('#content-wrapper');
                this._body = new ria.dom.Dom('body');
            },

            OVERRIDE, VOID, function onResume_() {
                BASE();
                this.dom.removeClass(UNDER_OVERLAY_CLASS);
            },

            OVERRIDE, VOID, function onPause_() {
                this.dom.addClass(UNDER_OVERLAY_CLASS);
                BASE();
            },


            Date, function getDate(str_, a_, b_){
                return chlk.models.common.ChlkDate.GET_SERVER_DATE(str_, a_, b_);
            },

            Date, function getSchoolYearServerDate(str_, a_, b_){
                return chlk.models.common.ChlkSchoolYearDate.GET_SCHOOL_YEAR_SERVER_DATE(str_, a_, b_);
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

            OVERRIDE, VOID, function onRender_(model) {
                BASE(model);

                var target = ria.dom.Dom('#classes-bar-holder').empty();

                this.dom.find('> div > form > .classes-bar, > div > form > .grades-bar').removeSelf().appendTo(target);
                this.dom.find('> div > .classes-bar, > div > .grades-bar').removeSelf().appendTo(target);
                this.dom.find('> .classes-bar, > .grades-bar').removeSelf().appendTo(target);
            },

            OVERRIDE, VOID, function onPartialRender_(model, msg_) {
                BASE(model, msg_);

                var $classesBar = this.dom.find('.classes-bar, .grades-bar');
                if ($classesBar.exists()) {
                    $classesBar.removeSelf().appendTo(ria.dom.Dom('#classes-bar-holder').empty());
                }
            }
        ]);
});
