REQUIRE('ria.mvc.DomControl');
REQUIRE('chlk.models.common.Role');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.Base */
    CLASS(
        'Base', EXTENDS(ria.mvc.Control), [

            function $() {
                BASE();
                this._dom = null;
                this._domEvents = [];

                this.bind_();
            },

            OVERRIDE, VOID, function onCreate_() {
                BASE();

                var dom = this._dom = new ria.dom.Dom();

                var instance = this;
                this._domEvents.forEach(function (_) {
                    dom.on(_.event, _.selector || null, _.wrapper || (_.wrapper = function (node, event) {
                        return _.methodRef.invokeOn(instance, ria.__API.clone(arguments));
                    }));
                })
            },

            VOID, function bind_() {
                var ref = new ria.reflection.ReflectionClass(this.getClass());

                this._domEvents = [].concat.apply([], ref.getMethodsReflector()
                    .filter(function (_) { return _.isAnnotatedWith(ria.mvc.DomEventBind)})
                    .map(function(_) {
                        if (_.getArguments().length < 2)
                            throw new ria.mvc.MvcException('Methods, annotated with ria.mvc.DomBindEvent, are expected to accept at least two arguments (node, event)');

                        return _.findAnnotation(ria.mvc.DomEventBind)
                            .map(function (annotation) {
                                return {
                                    event: annotation.event,
                                    selector: annotation.selector_,
                                    methodRef: _
                                }
                            });
                    }));
            },

            chlk.models.common.Role, function getUserRole(){
                return this.getContext().getSession().get(ChlkSessionConstants.USER_ROLE);
            },

            Date, function getServerDate(str_, a_, b_){
                return chlk.models.common.ChlkDate.GET_SERVER_DATE(str_, a_, b_);
            },

            Date, function getSchoolYearServerDate(str_, a_, b_){
                return chlk.models.common.ChlkSchoolYearDate.GET_SCHOOL_YEAR_SEVER_DATE(str_, a_, b_);
            },
        ]);
});