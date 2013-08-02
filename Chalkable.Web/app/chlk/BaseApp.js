REQUIRE('ria.mvc.Application');
REQUIRE('ria.dom.jQueryDom');
REQUIRE('ria.dom.ready');

REQUIRE('chlk.controls.ActionFormControl');
REQUIRE('chlk.controls.ActionLinkControl');
REQUIRE('chlk.controls.AvatarControl');
REQUIRE('chlk.controls.ButtonControl');
REQUIRE('chlk.controls.CheckboxControl');
REQUIRE('chlk.controls.DatePickerControl');
REQUIRE('chlk.controls.FileUploadControl');
REQUIRE('chlk.controls.GlanceBoxControl');
REQUIRE('chlk.controls.GridControl');
REQUIRE('chlk.controls.LeftRightToolbarControl');
REQUIRE('chlk.controls.ListViewControl');
REQUIRE('chlk.controls.LoadingImgControl');
REQUIRE('chlk.controls.PaginatorControl');
REQUIRE('chlk.controls.PhotoContainerControl');
REQUIRE('chlk.controls.SelectControl');

REQUIRE('chlk.models.common.Role');
REQUIRE('chlk.models.schoolYear.MarkingPeriod');

NAMESPACE('chlk', function (){

    var logonShowed = false;

    /*new ria.dom.Dom().on('click', '.action-bar .action-button:not(.pressed), .action-bar .action-link:not(.pressed)', function(node, event){
        node.parent('.action-bar')
            .find('.pressed')
            .removeClass('pressed');
        node.addClass('pressed');
    });*/

    new ria.dom.Dom('.logout-area').on('click', function(node, event){
        var elem = node.parent().find('a');
        if(!logonShowed){
            elem.setCss("visibility", "visible")
                .setCss("opacity", 1);
        }else{
            elem.setCss("opacity", 0);
            setTimeout(function(){
                elem.setCss("visibility", "hidden");
            }, 200);
        }
        logonShowed = !logonShowed;
    });

    /** @class chlk.BaseApp */
    CLASS(
        'BaseApp', EXTENDS(ria.mvc.Application), [
            function $(){
                BASE();
                var serializer = new ria.serialize.JsonSerializer();
                window.markingPeriod && this.getContext().getSession().set('markingPeriod', serializer.deserialize(window.markingPeriod, chlk.models.schoolYear.MarkingPeriod));
                window.nextMarkingPeriod && this.getContext().getSession().set('nextMarkingPeriod', serializer.deserialize(window.nextMarkingPeriod, chlk.models.schoolYear.MarkingPeriod));
                window.finalizedClassesIds && this.getContext().getSession().set('finalizedClassesIds', window.finalizedClassesIds);
                window.currentChlkPerson && this.getContext().getSession().set('currentPerson', serializer.deserialize(window.currentChlkPerson, chlk.models.people.Person));
            }
        ]);
});
