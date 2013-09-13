REQUIRE('ria.mvc.MvcException');
REQUIRE('ria.mvc.Activity');
REQUIRE('ria.dom.Dom');
REQUIRE('ria.mvc.DomEventBind');

REQUIRE('ria.reflection.ReflectionClass');

window.noLoadingMsg = 'no-loading';

NAMESPACE('ria.mvc', function () {

    var MODEL_WAIT_CLASS = 'activity-model-wait';

    function camel2dashed(_) {
        return _.replace(/[a-z][A-Z]/g, function(str, offset) {
           return str[0] + '-' + str[1].toLowerCase();
        });
    }

    var MODEL_WAIT_CLASS = 'activity-model-wait';

    function camel2dashed(_) {
        return _.replace(/[a-z][A-Z]/g, function(str, offset) {
           return str[0] + '-' + str[1].toLowerCase();
        });
    }

    /** @class ria.mvc.DomAppendTo */
    ANNOTATION(
        [[String]],
        function DomAppendTo(node) {});

    /** @class ria.mvc.DomActivity */
    CLASS(
        'DomActivity', EXTENDS(ria.mvc.Activity), [
            ria.dom.Dom, 'dom',

            function $() {
                BASE();

                this._actitivyClass = null;
                this._domAppendTo = null;
                this._domEvents = [];
                this.processAnnotations_(new ria.reflection.ReflectionClass(this.getClass()));
            },

            [[ria.reflection.ReflectionClass]],
            VOID, function processAnnotations_(ref) {
                this._activityClass = camel2dashed(ref.getShortName());

                if (!ref.isAnnotatedWith(ria.mvc.DomAppendTo))
                    throw new ria.mvc.MvcException('ria.mvc.DomActivity expects annotation ria.mvc.DomAppendTo');

                this._domAppendTo = new ria.dom.Dom(ref.findAnnotation(ria.mvc.DomAppendTo).pop().node);

                this._domEvents = ref.getMethodsReflector()
                    .filter(function (_) { return _.isAnnotatedWith(ria.mvc.DomEventBind)})
                    .map(function(_) {
                        if (_.getArguments().length < 2)
                            throw new ria.mvc.MvcException('Methods, annotated with ria.mvc.DomBindEvent, are expected to accept at least two arguments (node, event)');

                        var annotation = _.findAnnotation(ria.mvc.DomEventBind).pop();
                        return {
                            event: annotation.event,
                            selector: annotation.selector_,
                            methodRef: _
                        }
                    })
            },

            ABSTRACT, ria.dom.Dom, function onDomCreate_() {},

            OVERRIDE, VOID, function onCreate_() {
                BASE();

                var dom = this.dom = this.onDomCreate_().addClass(this._actitivyClass);

                var instance = this;
                this._domEvents.forEach(function (_) {
                    dom.on(_.event, _.selector || null, _.wrapper || (_.wrapper = function (node, event) {
                        return _.methodRef.invokeOn(instance, ria.__API.clone(arguments));
                    }));
                })
            },

            OVERRIDE, VOID, function onStart_() {
                BASE();
                this.dom.appendTo(this._domAppendTo);
            },

            OVERRIDE, VOID, function onStop_(){
                BASE();
                this._domAppendTo.remove(this.dom.empty());
            },

            ///TODO: WTF?????
            OVERRIDE, VOID, function startLoading() {
                this.dom.addClass(MODEL_WAIT_CLASS);
            },

            ///TODO: WTF?????
            OVERRIDE, VOID, function stopLoading() {
                this.dom.removeClass(MODEL_WAIT_CLASS);
            },

            [[String]],
            OVERRIDE, VOID, function onModelWait_(msg_) {
                BASE(msg_);
                msg_ != window.noLoadingMsg && this.startLoading();
            },
            [[Object, String]],
            OVERRIDE, VOID, function onModelError_(data, msg_) {
                BASE(data, msg_);
                this.stopLoading();
            },
            [[Object]],
            OVERRIDE, VOID, function onRefresh_(data) {
                BASE(data);
                this.stopLoading();
            },
            OVERRIDE, VOID, function onPartialRefresh_(data, msg_) {
                BASE(data, msg_);
                this.stopLoading();
            }
        ]);
});