REQUIRE('ria.mvc.Application');
REQUIRE('ria.dom.Dom');
REQUIRE('ria.dom.ready');

REQUIRE('chlk.controllers.SysAdminController');
REQUIRE('chlk.controls.ActionLinkControl');
REQUIRE('chlk.controls.GridControl');
REQUIRE('chlk.controls.PaginatorControl');

NAMESPACE('chlk', function (){

    /** @class chlk.SysAdminApp */
    CLASS(
        'SysAdminApp', EXTENDS(ria.mvc.Application), [
            OVERRIDE, ria.async.Future, function onStart_() {
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
