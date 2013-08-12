REQUIRE('ria.mvc.Application');
REQUIRE('ria.dom.jQueryDom');
REQUIRE('ria.dom.ready');

REQUIRE('chlk.controls.ActionFormControl');
REQUIRE('chlk.controls.LabeledCheckboxControl');
REQUIRE('chlk.controls.SlideCheckboxControl');
REQUIRE('chlk.controls.ActionLinkControl');
REQUIRE('chlk.controls.AvatarControl');
REQUIRE('chlk.controls.ButtonControl');
REQUIRE('chlk.controls.CheckboxControl');
REQUIRE('chlk.controls.CheckboxListControl');
REQUIRE('chlk.controls.DatePickerControl');
REQUIRE('chlk.controls.SelectControl');
REQUIRE('chlk.controls.DateSelectControl');
REQUIRE('chlk.controls.FileUploadControl');
REQUIRE('chlk.controls.GlanceBoxControl');
REQUIRE('chlk.controls.GridControl');
REQUIRE('chlk.controls.LeftRightToolbarControl');
REQUIRE('chlk.controls.ListViewControl');
REQUIRE('chlk.controls.LoadingImgControl');
REQUIRE('chlk.controls.PaginatorControl');
REQUIRE('chlk.controls.PhotoContainerControl');
REQUIRE('chlk.controls.LogoutControl');

REQUIRE('chlk.models.common.Role');
REQUIRE('chlk.models.schoolYear.MarkingPeriod');

NAMESPACE('chlk', function (){



    /** @class chlk.BaseApp */
    CLASS(
        'BaseApp', EXTENDS(ria.mvc.Application), [
            function $(){
                BASE();
                var serializer = new ria.serialize.JsonSerializer();
                window.markingPeriod && this.getContext().getSession().set('markingPeriod', serializer.deserialize(window.markingPeriod, chlk.models.schoolYear.MarkingPeriod));
                window.nextMarkingPeriod && this.getContext().getSession().set('nextMarkingPeriod', serializer.deserialize(window.nextMarkingPeriod, chlk.models.schoolYear.MarkingPeriod));
                window.finalizedClassesIds && this.getContext().getSession().set('finalizedClassesIds', window.finalizedClassesIds);
                window.currentChlkPerson && this.getContext().getSession().set('currentPerson', serializer.deserialize(window.currentChlkPerson, chlk.models.people.User));
                window.WEB_SITE_ROOT && this.getContext().getSession().set('webSiteRoot', window.WEB_SITE_ROOT);
                this.getContext().getSession().set('azurePictureUrl', window.azurePictureUrl);
            },

            function getCurrentPerson(){
                return this.getContext().getSession().get('currentPerson');
            },

            OVERRIDE, ria.async.Future, function onStart_() {
                return BASE()
                    .then(function(data){
                        new ria.dom.Dom()
                            .fromHTML(ASSET('~/assets/jade/common/logout.jade')(this))
                            .appendTo("#logout-block");
                        return data;
                    }, this);
            }


        ]);
});
