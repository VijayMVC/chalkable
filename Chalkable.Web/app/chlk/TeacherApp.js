REQUIRE('ria.mvc.Application');
REQUIRE('ria.dom.jQueryDom');
REQUIRE('ria.dom.ready');

REQUIRE('chlk.controllers.FeedController');


REQUIRE('chlk.controls.ActionLinkControl');

NAMESPACE('chlk', function (){

    /** @class chlk.TeacherApp */
    CLASS(
        'TeacherApp', EXTENDS(ria.mvc.Application), [
            OVERRIDE, ria.mvc.Dispatcher, function initDispatcher_() {
                var dispatcher = BASE();

                dispatcher.setDefaultControllerId('feed');
                dispatcher.setDefaultControllerAction('list');
                return dispatcher;
            },
            OVERRIDE, ria.async.Future, function onStart_() {
                return BASE()
                    .then(function(data){
                        new ria.dom.Dom()
                            .fromHTML(ASSET('~/assets/jade/sidebars/TeacherSidebar.jade')())
                            .appendTo("#sidebar");
                        return data;
                    });
            }
        ]);
});
