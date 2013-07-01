REQUIRE('ria.mvc.Application');
REQUIRE('ria.dom.Dom');
REQUIRE('ria.dom.ready');

REQUIRE('chlk.controllers.SysAdminController');
REQUIRE('chlk.controllers.SchoolsController');
REQUIRE('chlk.controls.ActionLinkControl');
REQUIRE('chlk.controls.GridControl');
REQUIRE('chlk.controls.PaginatorControl');
REQUIRE('chlk.controls.ActionFormControl');
REQUIRE('chlk.controls.ButtonControl');
REQUIRE('chlk.controls.CheckboxControl');
REQUIRE('chlk.controls.GlanceBoxControl');
REQUIRE('chlk.controls.SelectControl');
REQUIRE('chlk.controls.AvatarControl');
REQUIRE('chlk.controls.PhotoContainerControl');

NAMESPACE('chlk', function (){

    /** @class chlk.SysAdminApp */
    CLASS(
        'SysAdminApp', EXTENDS(ria.mvc.Application), [
            OVERRIDE, ria.mvc.Dispatcher, function initDispatcher_() {
                var dispatcher = BASE();

                dispatcher.setDefaultControllerId('sysadmin');
                dispatcher.setDefaultControllerAction('schools');
                return dispatcher;
            },
            OVERRIDE, ria.async.Future, function onStart_() {
                var serializer = new ria.serialize.JsonSerializer();
                window.gradeLevels = serializer.deserialize([
                    {name: '1st', id: 1},
                    {name: '2st', id: 2},
                    {name: '3st', id: 3},
                    {name: '4st', id: 4},
                    {name: '5st', id: 5},
                    {name: '6st', id: 6},
                    {name: '7st', id: 7},
                    {name: '8st', id: 8},
                    {name: '9st', id: 9},
                    {name: '10st', id: 10},
                    {name: '11st', id: 11},
                    {name: '12st', id: 12}
                ], ArrayOf(chlk.models.NameId));

                return BASE()
                    .then(function (data) {
                        new ria.dom.Dom()
                            .fromHTML(ASSET('~/assets/jade/SysAdmin.jade')())
                            .appendTo('#main');

                        return data;
                    }).then(function(data){
                        new ria.dom.Dom()
                            .fromHTML(ASSET('~/assets/jade/sidebars/SysAdminSidebar.jade')())
                            .appendTo("#sidebar");
                        return data;
                    });
            }
        ]);
});
