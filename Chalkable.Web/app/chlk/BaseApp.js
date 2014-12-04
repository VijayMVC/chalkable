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
REQUIRE('chlk.controls.ProfileLinkControl');
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
REQUIRE('chlk.controls.SimplePayCheckControl');

REQUIRE('chlk.models.grading.GradeLevel');
REQUIRE('chlk.models.common.Role');
REQUIRE('chlk.models.common.AlertInfo');
REQUIRE('chlk.models.schoolYear.MarkingPeriod');
REQUIRE('chlk.models.attendance.AttendanceReason');
REQUIRE('chlk.models.id.SchoolYearId');
REQUIRE('chlk.models.people.Claim');
REQUIRE('chlk.models.grading.Mapping');
REQUIRE('chlk.models.grading.AlternateScore');
REQUIRE('chlk.models.grading.AlphaGrade');
REQUIRE('chlk.models.schoolYear.GradingPeriod');
REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.grading.AvgComment');
REQUIRE('chlk.models.school.SchoolOption');

REQUIRE('chlk.models.id.SchoolId');


REQUIRE('chlk.templates.common.AlertsPopUpTpl');

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

            OVERRIDE, VOID, function run() {
                var url = this.context.getSession().get(ChlkSessionConstants.REDIRECT_URL, false);
                if(url)
                    window.location.hash = url.substr(url.indexOf('#') + 1);

                BASE();
            },

            OVERRIDE, ria.mvc.ISession, function initSession_() {
                var session = BASE();
                window.currentChlkPerson.claims = window.userClaims;
                this.saveInSession(session, ChlkSessionConstants.MARKING_PERIOD, chlk.models.schoolYear.MarkingPeriod);
                this.saveInSession(session, ChlkSessionConstants.MARKING_PERIODS, ArrayOf(chlk.models.schoolYear.MarkingPeriod));
                this.saveInSession(session, ChlkSessionConstants.GRADING_PERIOD, chlk.models.schoolYear.GradingPeriod);
                this.saveInSession(session, ChlkSessionConstants.NEXT_MARKING_PERIOD, chlk.models.schoolYear.MarkingPeriod);
                this.saveInSession(session, ChlkSessionConstants.FINALIZED_CLASS_IDS);
                this.saveInSession(session, ChlkSessionConstants.CURRENT_CHLK_PERSON, chlk.models.people.User, ChlkSessionConstants.CURRENT_PERSON);

                this.saveInSession(session, 'WEB_SITE_ROOT', null, 'webSiteRoot');
                this.saveInSession(session, ChlkSessionConstants.AZURE_PICTURE_URL);
                this.saveInSession(session, ChlkSessionConstants.DEMO_AZURE_PICTURE_URL);
                this.saveInSession(session, ChlkSessionConstants.CURRENT_SCHOOL_YEAR_ID, chlk.models.id.SchoolYearId);
                this.saveInSession(session, ChlkSessionConstants.ATTENDANCE_REASONS, ArrayOf(chlk.models.attendance.AttendanceReason));
                this.saveInSession(session, ChlkSessionConstants.USER_CLAIMS, ArrayOf(chlk.models.people.Claim));
                this.saveInSession(session, ChlkSessionConstants.ALPHA_GRADES, ArrayOf(chlk.models.grading.AlphaGrade));
                this.saveInSession(session, ChlkSessionConstants.ALTERNATE_SCORES, ArrayOf(chlk.models.grading.AlternateScore));
                this.saveInSession(session, ChlkSessionConstants.NEW_NOTIFICATIONS, Number);
                this.saveInSession(session, ChlkSessionConstants.GRADING_COMMENTS, ArrayOf(chlk.models.grading.AvgComment));
                this.saveInSession(session, ChlkSessionConstants.SCHOOL_OPTIONS, chlk.models.school.SchoolOption);
                this.saveInSession(session, ChlkSessionConstants.DEMO_SCHOOL, Boolean);
                this.saveInSession(session, ChlkSessionConstants.FIRST_LOGIN, Boolean);
                this.saveInSession(session, ChlkSessionConstants.REDIRECT_URL, String);
                this.saveInSession(session, ChlkSessionConstants.DEMO_SCHOOL_PICTURE_DISTRICT, chlk.models.id.SchoolId);
                this.saveInSession(session, ChlkSessionConstants.CLASSES_TO_FILTER, ArrayOf(chlk.models.classes.ClassForTopBar));

                var newClasses = session.get(ChlkSessionConstants.CLASSES_TO_FILTER, []).slice();
                newClasses.unshift(chlk.lib.serialize.ChlkJsonSerializer().deserialize({
                    name: 'All',
                    description: 'All',
                    id: ''
                }, chlk.models.classes.ClassForTopBar));

                session.set(ChlkSessionConstants.CLASSES_TO_FILTER_WITH_ALL, newClasses);
                if(window.redirectUrl && window.redirectUrl.indexOf('setup/hello') > -1){
                    ria.dom.Dom('body').addClass('setup');
                }else{
                    ria.dom.Dom('#first-login-video').remove();
                }

                window.gradeLevels = window.gradeLevels || [];
                window.gradeLevels.forEach(function(item){
                    var numberPart = parseInt(item.name, 10);
                    if(numberPart){
                        item.serialPart = getSerial(numberPart).slice(numberPart.toString().length);
                        item.name = numberPart;
                    }
                    item.fullText = numberPart ? item.name + item.serialPart : item.name.trim();
                });
                this.saveInSession(session, ChlkSessionConstants.GRADE_LEVELS, ArrayOf(chlk.models.grading.GradeLevel));

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
                return this.getContext().getSession().get(ChlkSessionConstants.CURRENT_PERSON, null);
            },

            OVERRIDE, ria.async.Future, function onStart_() {

                ria.templates.ConverterFactories().register(new chlk.ConvertersFactory());

                //TODO Remove jQuery
                jQuery(document).on('mouseover mousemove', '[data-tooltip]', function(e){
                    if(!jQuery(this).data('wasClick')){
                        var target = jQuery(e.target),
                            tooltip = jQuery('#chlk-tooltip-item');
                        target.off('remove.tooltip');
                        target.on('remove.tooltip', function(e){
                            target.trigger('mouseleave');
                        });
                        if(target.hasClass('no-tooltip') || target.parents('.no-tooltip')[0]){
                            tooltip.hide();
                            tooltip.find('.tooltip-content').html('');
                        }else{
                            var node = jQuery(this),
                                offset = node.offset(),
                                showTooltip = true;
                            var value = node.data('tooltip');
                            var type = node.data('tooltip-type');
                            var clazz = node.data('tooltip-class');
                            if(type == "overflow"){
                                showTooltip = this.scrollWidth > (node.width() + parseInt(node.css('padding-left'), 10) + parseInt(node.css('padding-right'), 10));
                            }
                            if(value && showTooltip){
                                tooltip.show();
                                tooltip.find('.tooltip-content').html(node.data('tooltip'));
                                tooltip.css('left', offset.left + (node.width() - tooltip.width())/2)
                                    .css('top', offset.top - tooltip.height());
                                clazz && tooltip.addClass(clazz);
                                e.stopPropagation();
                            }
                        }
                    }

                });

                jQuery(document).on('mouseleave click', '[data-tooltip]', function(e){
                    var node = jQuery(this);
                    if(e.type == "click"){
                        node.data('wasClick', true);
                        setTimeout(function(){
                            node.data('wasClick', null);
                        }, 100)
                    }
                    var clazz = node.data('tooltip-class');
                    var tooltip = jQuery('#chlk-tooltip-item');
                    tooltip.hide();
                    clazz && tooltip.removeClass(clazz);
                    tooltip.find('.tooltip-content').html('');
                });

                jQuery(document).on('mouseover mousemove', '.alerts-icon', function(e){
                    if(!jQuery(this).data('wasClick')) {
                        var tooltip = jQuery('.alerts-pop-up'),
                            node = jQuery(this),
                            offset = node.offset();
                        var o = {
//                            isallowedinetaccess: !!node.data('allowedinetaccess'),
//                            hasmedicalalert: !!node.data('withmedicalalert'),
//                            specialinstructions: node.data('specialinstructions'),
//                            spedstatus: node.data('spedstatus')
                            alerts: node.data('stringalerts')
                        };
                        var js = new ria.serialize.JsonSerializer();
                        //var model = new chlk.models.common.Alerts.$create(o.stringAlerts);
                        var model = js.deserialize(o, chlk.models.common.Alerts);

                        var alerts = model.getAlerts() || [];

                        if (alerts.length > 0){
                            var tpl = chlk.templates.common.AlertsPopUpTpl();
                            tpl.assign(model);
                            tooltip.html(tpl.render());
                            tooltip.show();
                            var top = offset.top - (tooltip.height() - node.height()) / 2;
                            tooltip.css('left', offset.left + node.width() + 20)
                                .css('top', top > 0 ? top : 0);
                            if(top < 0)
                                tooltip.find('.alerts-triangle').css('top', top);
                            e.stopPropagation();
                        }
                    }

                });

                jQuery(document).on('mouseleave click', '.alerts-icon', function(e){
                    if(e.type == "click"){
                        var node = jQuery(this);
                        node.data('wasClick', true);
                        setTimeout(function(){
                            node.data('wasClick', null);
                        }, 100)
                    }
                    var tooltip = jQuery('.alerts-pop-up');
                    tooltip.hide();
                    tooltip.html('');
                });

                ria.dom.Dom('#demo-footer')
                    .on('click', '[data-rolename]', function($node, event) {
                        if(!$node.hasClass('pressed')) {
                            window.location.href = WEB_SITE_ROOT + 'DemoSchool/LogOnIntoDemo.json'
                                + '?rolename=' + $node.getData('rolename')
                                + '&prefix=' + window.school.demoprefix;
                        }
                        return false;
                    });

                this.apiHost_.onStart(this.context);

                return BASE()
                    .then(function(data){
                        if(this.getCurrentPerson())
                            new ria.dom.Dom()
                                .fromHTML(ASSET('~/assets/jade/common/header.jade')(this))
                                .appendTo("header");
                        return data;
                    }, this);
            },

            OVERRIDE, VOID, function onStop_() {
                this.apiHost_.onStop();
            }

        ]);
});
