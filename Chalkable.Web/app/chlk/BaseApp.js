REQUIRE('ria.mvc.DefaultApplication');
REQUIRE('ria.dom.jQueryDom');
REQUIRE('ria.dom.ready');
REQUIRE('chlk.activities.lib.TemplatePage');

REQUIRE('chlk.controls.ActionFormControl');
REQUIRE('chlk.controls.ActionLinkControl');
REQUIRE('chlk.controls.AvatarControl');
REQUIRE('chlk.controls.ButtonControl');
REQUIRE('chlk.controls.CarouselControl');
REQUIRE('chlk.controls.ChartControl');
REQUIRE('chlk.controls.CheckboxControl');
REQUIRE('chlk.controls.CheckboxListControl');
REQUIRE('chlk.controls.DatePickerControl');
REQUIRE('chlk.controls.DateSelectControl');
REQUIRE('chlk.controls.FeedItemControl');
REQUIRE('chlk.controls.FileUploadControl');
REQUIRE('chlk.controls.GlanceBoxControl');
REQUIRE('chlk.controls.GridControl');
REQUIRE('chlk.controls.ImageControl');
REQUIRE('chlk.controls.LabeledCheckboxControl');
REQUIRE('chlk.controls.LeftRightToolbarControl');
REQUIRE('chlk.controls.ListForWeekCalendarControl');
REQUIRE('chlk.controls.ListViewControl');
REQUIRE('chlk.controls.LoadingImgControl');
REQUIRE('chlk.controls.LogoutControl');
REQUIRE('chlk.controls.PaginatorControl');
REQUIRE('chlk.controls.PhotoContainerControl');
REQUIRE('chlk.controls.RangeSliderControl');
REQUIRE('chlk.controls.SearchBoxControl');
REQUIRE('chlk.controls.SelectControl');
REQUIRE('chlk.controls.SlideCheckboxControl');
REQUIRE('chlk.controls.TextAreaControl');
REQUIRE('chlk.controls.VideoControl');
REQUIRE('chlk.controls.PayCheckControl');
REQUIRE('chlk.controls.ScrollBoxControl');
REQUIRE('chlk.controls.MultipleSelectControl');
REQUIRE('chlk.controls.MaskedInputControl');

REQUIRE('chlk.models.grading.GradeLevel');
REQUIRE('chlk.models.common.Role');
REQUIRE('chlk.models.schoolYear.MarkingPeriod');
REQUIRE('chlk.models.attendance.AttendanceReason');
REQUIRE('chlk.models.id.SchoolYearId');
REQUIRE('chlk.models.people.Claim');
REQUIRE('chlk.models.grading.Mapping');
REQUIRE('chlk.models.grading.AlternateScore');
REQUIRE('chlk.models.grading.AlphaGrade');


REQUIRE('chlk.AppApiHost');
REQUIRE('chlk.lib.serialize.ChlkJsonSerializer');
REQUIRE('chlk.lib.mvc.ChlkView');
REQUIRE('chlk.controllers.ErrorController');

NAMESPACE('chlk', function (){

    /** @class chlk.ConvertersFactory */
    CLASS(
        'ConvertersFactory', IMPLEMENTS(ria.templates.IConverterFactory), [
            [[ImplementerOf(ria.templates.IConverter)]],
            Boolean, function canCreate(converterClass) {
                return true;
            },

            [[ImplementerOf(ria.templates.IConverter)]],
            ria.templates.IConverter, function create(converterClass) {
                return new converterClass();
            }
        ]);

    /** @class chlk.BaseApp */
    CLASS(
        'BaseApp', EXTENDS(ria.mvc.DefaultApplication), [
            function $(){
                BASE();
                this.apiHost_ = new chlk.AppApiHost();
            },

            OVERRIDE, ria.mvc.IView, function initView_() {
                return new chlk.lib.mvc.ChlkView;
            },

            OVERRIDE, ria.mvc.ISession, function initSession_() {
                var session = BASE();
                window.currentChlkPerson.claims = window.userClaims;
                this.saveInSession(session, 'markingPeriod', chlk.models.schoolYear.MarkingPeriod);
                this.saveInSession(session, 'gradingPeriod', chlk.models.schoolYear.GradingPeriod);
                this.saveInSession(session, 'nextMarkingPeriod', chlk.models.schoolYear.MarkingPeriod);
                this.saveInSession(session, 'finalizedClassesIds');
                this.saveInSession(session, 'currentChlkPerson', chlk.models.people.User, 'currentPerson');
                this.saveInSession(session, 'WEB_SITE_ROOT', null, 'webSiteRoot');
                this.saveInSession(session, 'azurePictureUrl');
                this.saveInSession(session, 'currentSchoolYearId', chlk.models.id.SchoolYearId);
                this.saveInSession(session, 'attendanceReasons', ArrayOf(chlk.models.attendance.AttendanceReason));
                this.saveInSession(session, 'userClaims', ArrayOf(chlk.models.people.Claim));
                this.saveInSession(session, 'alphaGrades', ArrayOf(chlk.models.grading.AlphaGrade));
                this.saveInSession(session, 'alternateScores', ArrayOf(chlk.models.grading.AlternateScore));
                this.saveInSession(session, 'newNotifications', Number);

                window.gradeLevels.forEach(function(item){
                    var numberPart = parseInt(item.name, 10);
                    if(numberPart){
                        item.serialPart = getSerial(numberPart).slice(numberPart.toString().length);
                        item.name = numberPart;
                    }
                    item.fullText = numberPart ? item.name + item.serialPart : item.name.trim();
                });
                this.saveInSession(session, 'gradeLevels', ArrayOf(chlk.models.grading.GradeLevel));

                var siteRoot = window.location.toString().split(window.location.pathname).shift();
                var serviceRoot = "/";
                session.set('siteRoot', siteRoot + serviceRoot);
                return session;
            },

            [[ria.mvc.ISession, String, Object, String]],
            function saveInSession(session, key, cls_, destKey_){
               var serializer = new chlk.lib.serialize.ChlkJsonSerializer();

               var defaultValue = {};
               if (cls_ && (ria.__API.isArrayOfDescriptor(cls_) || cls_ == Array))
                   defaultValue = [];
               if (cls_ && (ria.__API.isIdentifier(cls_)))
                   defaultValue = "";

               var value = window[key];
               if(value == undefined || value == null)
                  value = defaultValue;

               var destK = destKey_ ?  destKey_ : key;
               if (value != undefined && value != null){
                       cls_ ? session.set(destK, serializer.deserialize(value, cls_))
                            : session.set(destK, value);
               }
            },

            function getCurrentPerson(){
                return this.getContext().getSession().get('currentPerson', null);
            },

            OVERRIDE, ria.async.Future, function onStart_() {

                ria.templates.ConverterFactories().register(new chlk.ConvertersFactory());

                //TODO Remove jQuery
                jQuery(document).on('mouseover mousemove', '[data-tooltip]', function(e){
                    if(!jQuery(this).data('wasClick')){
                        var node = jQuery(this),
                            tooltip = jQuery('#chlk-tooltip-item'),
                            offset = node.offset(),
                            showTooltip = true;
                        var value = node.data('tooltip');
                        var type = node.data('tooltip-type');
                        if(type == "overflow"){
                            showTooltip = this.scrollWidth > (node.width() + parseInt(node.css('padding-left'), 10) + parseInt(node.css('padding-right'), 10));
                        }
                        if(value && showTooltip){
                            tooltip.show();
                            tooltip.find('.tooltip-content').html(node.data('tooltip'));
                            tooltip.css('left', offset.left + (node.width() - tooltip.width())/2)
                                .css('top', offset.top - tooltip.height());
                            e.stopPropagation();
                        }
                    }

                });

                jQuery(document).on('mouseleave click', '[data-tooltip]', function(e){
                    if(e.type == "click"){
                        var node = jQuery(this);
                        node.data('wasClick', true);
                        setTimeout(function(){
                            node.data('wasClick', null);
                        }, 100)
                    }
                    var tooltip = jQuery('#chlk-tooltip-item');
                    tooltip.hide();
                    tooltip.find('.tooltip-content').html('');
                });

                 jQuery(document).on('click', '.demo-role-button:not(.coming)', function(){
                    if(!jQuery(this).hasClass('active')){
                        window.location.href = WEB_SITE_ROOT + 'DemoSchool/LogOnIntoDemo.json?rolename='
                            + jQuery(this).attr('rolename') + '&prefix=' + window.school.demoprefix;
                    }
                    return false;
                });

                this.apiHost_.onStart(this.context);

                return BASE()
                    .then(function(data){
                        if(this.getCurrentPerson())
                            new ria.dom.Dom()
                                .fromHTML(ASSET('~/assets/jade/common/logout.jade')(this))
                                .appendTo("#logout-block");
                        return data;
                    }, this);
            },

            OVERRIDE, VOID, function onStop_() {
                this.apiHost_.onStop();
            }

        ]);
});
