REQUIRE('ria.mvc.Application');
REQUIRE('ria.dom.jQueryDom');
REQUIRE('ria.dom.ready');

REQUIRE('chlk.controls.ActionLinkControl');
REQUIRE('chlk.controls.ListViewControl');
REQUIRE('chlk.controls.GridControl');
REQUIRE('chlk.controls.PaginatorControl');
REQUIRE('chlk.controls.ActionFormControl');
REQUIRE('chlk.controls.ButtonControl');
REQUIRE('chlk.controls.CheckboxControl');
REQUIRE('chlk.controls.GlanceBoxControl');
REQUIRE('chlk.controls.SelectControl');
REQUIRE('chlk.controls.AvatarControl');
REQUIRE('chlk.controls.PhotoContainerControl');
REQUIRE('chlk.controls.LeftRightToolbarControl');

REQUIRE('chlk.models.common.Role');

NAMESPACE('chlk', function (){

    new ria.dom.Dom().on('click', '.action-bar .action-button:not(.pressed), .action-bar .action-link:not(.pressed)', function(node, event){
        node.parent('.action-bar')
            .find('.pressed')
            .removeClass('pressed');
        node.addClass('pressed');
    });

    /** @class chlk.BaseApp */
    CLASS(
        'BaseApp', EXTENDS(ria.mvc.Application), [
        ]);
});
