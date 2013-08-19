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
REQUIRE('chlk.controls.VideoControl');
REQUIRE('chlk.controls.LogoutControl');
REQUIRE('chlk.controls.TextAreaControl');

REQUIRE('chlk.models.common.Role');
REQUIRE('chlk.models.schoolYear.MarkingPeriod');

NAMESPACE('chlk', function (){

    /** @class chlk.BaseApp */
    CLASS(
        'BaseApp', EXTENDS(ria.mvc.Application), [
            function $(){
                BASE();

            },

            OVERRIDE, ria.mvc.ISession, function initSession_() {
                var session = BASE();
                this.saveInSession(session, 'markingPeriod', chlk.models.schoolYear.MarkingPeriod);
                this.saveInSession(session, 'nextMarkingPeriod', chlk.models.schoolYear.MarkingPeriod);
                this.saveInSession(session, 'finalizedClassesIds');
                this.saveInSession(session, 'currentChlkPerson', chlk.models.people.User, 'currentPerson');
                this.saveInSession(session, 'WEB_SITE_ROOT', null, 'webSiteRoot');
                this.saveInSession(session, 'azurePictureUrl');

                return session;
            },

            [[ria.mvc.ISession, String, Object, String]],
            function saveInSession(session, key, cls_, destKey_){
               var serializer = new ria.serialize.JsonSerializer();
               var value = window[key] || {};

               var destK = destKey_ ?  destKey_ : key;
               if (value){
                       cls_ ? session.set(destK, serializer.deserialize(value, cls_))
                            : session.set(destK, value);
               }
            },

            function getCurrentPerson(){
                return this.getContext().getSession().get('currentPerson', null);
            },

            OVERRIDE, ria.async.Future, function onStart_() {

                //TODO Remove jQuery
                jQuery(document).on('mouseover', '[data-tooltip]', function(){
                    var node = jQuery(this), tooltip = jQuery('#chlk-tooltip-item'), offset = node.offset();
                    var value = node.data('tooltip');
                    if(value){
                        tooltip.show();
                        tooltip.find('.tooltip-content').html(node.data('tooltip'));
                        tooltip.css('left', offset.left + (node.width() - tooltip.width())/2)
                            .css('top', offset.top - tooltip.height());
                    }
                });

                jQuery(document).on('mouseleave', '[data-tooltip]', function(){
                    var tooltip = jQuery('#chlk-tooltip-item');
                    tooltip.hide();
                    tooltip.find('.tooltip-content').html('');
                });


                return BASE()
                    .then(function(data){
                        if(this.getCurrentPerson())
                            new ria.dom.Dom()
                                .fromHTML(ASSET('~/assets/jade/common/logout.jade')(this))
                                .appendTo("#logout-block");
                        return data;
                    }, this);
            }


        ]);
});
