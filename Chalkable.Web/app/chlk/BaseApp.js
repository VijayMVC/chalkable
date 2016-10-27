REQUIRE('ria.mvc.DefaultApplication');
REQUIRE('chlk.lib.dom.jQueryDom');
REQUIRE('ria.dom.ready');
REQUIRE('chlk.activities.lib.TemplatePage');

REQUIRE('chlk.controls.ActionFormControl');
REQUIRE('chlk.controls.ActionLinkControl');
REQUIRE('chlk.controls.AvatarControl');
REQUIRE('chlk.controls.ButtonControl');
REQUIRE('chlk.controls.CarouselControl');
REQUIRE('chlk.controls.ChartControl');
REQUIRE('chlk.controls.ActionCheckboxControl');
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
REQUIRE('chlk.controls.ScrollBoxControl');
REQUIRE('chlk.controls.MultipleSelectControl');
REQUIRE('chlk.controls.CloseOpenControl');
REQUIRE('chlk.controls.ClassesBarControl');
REQUIRE('chlk.controls.GradesBarControl');
REQUIRE('chlk.controls.PanoramaFilterControl');
REQUIRE('chlk.controls.DoubleSelectControl');
REQUIRE('chlk.controls.TabsControl');
REQUIRE('chlk.controls.PdfDocumentControl');
REQUIRE('chlk.controls.GroupPeopleSelectorControl');

REQUIRE('chlk.models.grading.GradeLevel');
REQUIRE('chlk.models.common.Role');
REQUIRE('chlk.models.common.AlertInfo');
REQUIRE('chlk.models.schoolYear.MarkingPeriod');
REQUIRE('chlk.models.schoolYear.Year');
REQUIRE('chlk.models.attendance.AttendanceReason');
REQUIRE('chlk.models.id.SchoolYearId');
REQUIRE('chlk.models.people.Claim');
REQUIRE('chlk.models.grading.AlternateScore');
REQUIRE('chlk.models.grading.AlphaGrade');
REQUIRE('chlk.models.schoolYear.GradingPeriod');
REQUIRE('chlk.models.people.ShortUserInfo');
REQUIRE('chlk.models.grading.AvgComment');
REQUIRE('chlk.models.school.SchoolOption');
REQUIRE('chlk.models.school.LEParams');
REQUIRE('chlk.models.announcement.AnnouncementAttributeType');
REQUIRE('chlk.models.settings.AdminMessaging');

REQUIRE('chlk.models.id.SchoolId');
REQUIRE('chlk.models.id.DistrictId');


REQUIRE('chlk.templates.common.AlertsPopUpTpl');

REQUIRE('chlk.AppApiHost');
REQUIRE('chlk.lib.serialize.ChlkJsonSerializer');
REQUIRE('chlk.lib.mvc.ChlkView');
REQUIRE('chlk.controllers.ErrorController');
REQUIRE('chlk.controllers.AttachController');

NAMESPACE('chlk', function (){

    // FIX for IE
    (function(win_loc) {
        try {
            if (!win_loc.origin) {
                win_loc.origin = win_loc.protocol + "//" + win_loc.hostname + (win_loc.port ? ':' + win_loc.port : '');
            }
        } catch (ignored) {}
    })(window.location);

    var Raygun = window.Raygun || null;

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
                window.attendanceReasons.sort(function(a,b){return (a.name.toLowerCase() > b.name.toLowerCase()) ? 1 : (a.name.toLowerCase() < b.name.toLowerCase() ? -1 : 0) });

                this.saveInSession(session, ChlkSessionConstants.MARKING_PERIOD, chlk.models.schoolYear.MarkingPeriod);
                this.saveInSession(session, ChlkSessionConstants.MARKING_PERIODS, ArrayOf(chlk.models.schoolYear.MarkingPeriod));
                this.saveInSession(session, ChlkSessionConstants.GRADING_PERIOD, chlk.models.schoolYear.GradingPeriod);
                this.saveInSession(session, ChlkSessionConstants.GRADING_PERIODS, ArrayOf(chlk.models.schoolYear.GradingPeriod));
                this.saveInSession(session, ChlkSessionConstants.SCHOOL_YEAR, chlk.models.schoolYear.Year, null);
                this.saveInSession(session, ChlkSessionConstants.NEXT_MARKING_PERIOD, chlk.models.schoolYear.MarkingPeriod);
                this.saveInSession(session, ChlkSessionConstants.MESSAGING_SETTINGS, chlk.models.settings.AdminMessaging);
                this.saveInSession(session, ChlkSessionConstants.FINALIZED_CLASS_IDS);
                this.saveInSession(session, ChlkSessionConstants.CURRENT_CHLK_PERSON, chlk.models.people.User, ChlkSessionConstants.CURRENT_PERSON);

                this.saveInSession(session, 'WEB_SITE_ROOT', null, 'webSiteRoot');
                this.saveInSession(session, ChlkSessionConstants.AZURE_PICTURE_URL);
                this.saveInSession(session, ChlkSessionConstants.DEMO_AZURE_PICTURE_URL);
                this.saveInSession(session, ChlkSessionConstants.SCHOOL_YEAR, chlk.models.id.SchoolYearId, ChlkSessionConstants.CURRENT_SCHOOL_YEAR_ID, ['id']);
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
                this.saveInSession(session, ChlkSessionConstants.CLASSES_TO_FILTER, ArrayOf(chlk.models.classes.Class));
                this.saveInSession(session, ChlkSessionConstants.GRADE_LEVELS, ArrayOf(chlk.models.grading.GradeLevel));
                this.saveInSession(session, ChlkSessionConstants.STUDY_CENTER_ENABLED, Boolean);
                this.saveInSession(session, ChlkSessionConstants.LE_PARAMS, chlk.models.school.LEParams);
                this.saveInSession(session, ChlkSessionConstants.ASSESSMENT_ENABLED, Boolean);
                this.saveInSession(session, ChlkSessionConstants.ANNOUNCEMENT_ATTRIBUTES, ArrayOf(chlk.models.announcement.AnnouncementAttributeType));
                this.saveInSession(session, ChlkSessionConstants.MESSAGING_DISABLED, Boolean);
                this.saveInSession(session, ChlkSessionConstants.YEARS, ArrayOf(Number));
                this.saveInSession(session, ChlkSessionConstants.SIS_API_VERSION, String);
                this.saveInSession(session, ChlkSessionConstants.SCHOOL_NAME, String);
                this.saveInSession(session, ChlkSessionConstants.REPORT_CARDS_ENABLED, Boolean);

                this.saveClassesInfoInSession(session, ChlkSessionConstants.CLASSES_INFO);

                if(window.redirectUrl && window.redirectUrl.indexOf('setup/hello') > -1){
                    ria.dom.Dom('body').addClass('setup');
                }else{
                    ria.dom.Dom('#first-login-video').remove();
                }


                var siteRoot = window.location.toString().split(window.location.pathname).shift();
                var serviceRoot = "/";

                session.set('siteRoot', siteRoot + serviceRoot);
                return session;
            },

            [[ria.mvc.ISession, Object]],
            function saveClassesInfoInSession(session,  key){
                var resObj= {};
                var serializer = new chlk.lib.serialize.ChlkJsonSerializer();
                var classesInfo = window[key];
                if(classesInfo != undefined && classesInfo != null){
                    for(var i in classesInfo) {
                        resObj[i] = serializer.deserialize(classesInfo[i], chlk.models.classes.ClassForWeekMask);
                    }
                }
                session.set(key, resObj);
            },

            [[ria.mvc.ISession, String, Object, String, ArrayOf(String)]],
            function saveInSession(session, key, cls_, destKey_, keys_){
               var serializer = new chlk.lib.serialize.ChlkJsonSerializer();

               var defaultValue = {};
               if (cls_ && (ria.__API.isArrayOfDescriptor(cls_) || cls_ == Array))
                   defaultValue = [];
               if (cls_ && (ria.__API.isIdentifier(cls_)))
                   defaultValue = "";

               var value = window[key];
                if(keys_ && value){
                    keys_.forEach(function(item){
                        value = value[item];
                    });
                }

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

                var tooltipTimeOut;

                if(window.loginTimeOut){
                    var loginTimer, loginTimeOut = window.loginTimeOut * 1000;

                    function logoutHandler_(){
                        document.location.href = WEB_SITE_ROOT + 'User/LogOutWithRedirect.json';
                    }

                    loginTimer = setTimeout(logoutHandler_, loginTimeOut);

                    ria.dom.Dom().on('mousedown keydown keyup mousemove touchstart touchcancel touchmove scroll', function(node, event){
                        clearTimeout(loginTimer);

                        loginTimer = setTimeout(logoutHandler_, loginTimeOut);
                    });
                }

                jQuery.fn.extend({setItemsChecked : function(value){
                    var jNode;
                    jQuery(this).each(function(index, item){
                        jNode = jQuery(this);
                        if(!!item.getAttribute('checked') != !!value){
                            jNode.prop('checked', value);
                            value ? this.setAttribute('checked', 'checked') : this.removeAttribute('checked');
                            value && this.setAttribute('checked', 'checked');
                            var node = jNode.parent().find('.hidden-checkbox');
                            node.val(value);
                            node.data('value', value);
                            node.attr('data-value', value);
                        }
                    });
                }});

                jQuery.fn.extend({setChecked : function(value){
                    var jNode = this, item = this[0];
                    if(!!item.getAttribute('checked') != !!value){
                        jNode.prop('checked', value);
                        value ? item.setAttribute('checked', 'checked') : item.removeAttribute('checked');
                        value && item.setAttribute('checked', 'checked');
                        var node = jNode.parent().find('.hidden-checkbox');
                        node.val(value);
                        node.data('value', value);
                        node.attr('data-value', value);
                    }
                }});

                //TODO Remove jQuery
                jQuery(document).on('mouseover mousemove', '[data-tooltip]', function(e){
                    if(!jQuery(this).data('wasClick') && !tooltipTimeOut){
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
                                showTooltip = true,
                                value = node.data('tooltip'),
                                type = node.data('tooltip-type'),
                                clazz = node.data('tooltip-class'),
                                onBottom = node.data('tooltip-position') == "bottom";
                            if(type == "overflow"){
                                showTooltip = this.scrollWidth > (node.width() + parseInt(node.css('padding-left'), 10) + parseInt(node.css('padding-right'), 10));
                            }
                            if((value || value === 0) && showTooltip ){
                                var timeout = node.data('tooltip-timeout');
                                if(timeout){
                                    tooltipTimeOut = setTimeout(function(){
                                        tooltip.show();
                                        tooltipTimeOut = null;
                                    }, timeout)
                                }else
                                    tooltip.show();

                                tooltip.find('.tooltip-content').html(node.data('tooltip'));
                                tooltip.css('left', offset.left + (node.width() - tooltip.width())/2);

                                if(onBottom)
                                    tooltip.css('top', offset.top + node.height() + 20).addClass('bottom');
                                else
                                    tooltip.css('top', offset.top - tooltip.height());
                                clazz && tooltip.addClass(clazz);
                            }
                        }
                    }

                });

                jQuery(document).on('mouseleave click', '[data-tooltip]', function(e){
                    clearTimeout(tooltipTimeOut);
                    tooltipTimeOut = null;
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
                    tooltip.removeClass('bottom');
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
                            var top = offset.top - 8;
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

                ria.dom.Dom()
                    .on('click', '.box-checkbox', function($node, event) {
                        if(event.target.className.indexOf('box-checkbox') > -1)
                            $node.find('input[type=checkbox]').trigger('click')
                    });

                ria.dom.Dom()
                    .on('mousedown', '#search-glass', function($node, event) {
                        /*var select = jQuery('#siteSearch');
                        var val = select.val();
                        if(val)
                            jQuery('#siteSearch').autocomplete( "search", val)*/
                        return false;
                    });

                ria.dom.Dom('#demo-footer')
                    .on('click', '[data-rolename]', function($node, event) {
                        if(!$node.hasClass('pressed')) {
                            window.location.href = WEB_SITE_ROOT + 'DemoSchool/LogOnIntoDemo.json'
                                + '?rolename=' + $node.getData('rolename')
                                + '&prefix=' + window.districtId;
                        }
                        return false;
                    });

                ria.dom.Dom('#sidebar').on('click', '.action-link', function(){
                    var doc = ria.dom.Dom(), top = 85;
                    if(doc.scrollTop() > top)
                        doc.scrollTop(top)
                });

                this.apiHost_.onStart(this.context);

                return BASE()
                    .then(function(data){
                        if(this.getCurrentPerson())
                            new ria.dom.Dom()
                                .fromHTML(ASSET('~/assets/jade/common/header.jade')(this))
                                .appendTo("header");
                        return data;
                    }, this)
                    .then(function (data) {
                        ria.dom.Dom('#content').removeClass('loading')
                        return data;
                    });
            },

            function prepareSideBarOptions_(){
                var isStudyCenterEnabled = this.getContext().getSession().get(ChlkSessionConstants.STUDY_CENTER_ENABLED, false);
                var isMessagingDisabled = this.getContext().getSession().get(ChlkSessionConstants.MESSAGING_DISABLED, false);
                var leParams = this.getContext().getSession().get(ChlkSessionConstants.LE_PARAMS, new chlk.models.school.LEParams());
                var isAssessmentEnabled = this.getContext().getSession().get(ChlkSessionConstants.ASSESSMENT_ENABLED, false);
                var isPeopleEnabled = this.getContext().getSession().get(ChlkSessionConstants.USER_CLAIMS, []).filter(function(item){
                        return item.hasPermission(chlk.models.people.UserPermissionEnum.VIEW_STUDENT)
                            || item.hasPermission(chlk.models.people.UserPermissionEnum.VIEW_CLASSROOM_STUDENTS);
                    }).length > 0;

                var sidebarOptions = {
                    isAppStoreEnabled: isStudyCenterEnabled,
                    isLEEnabled: leParams.isLeEnabled(),
                    isLinkEnabled: leParams.isIntegratedSignOn(),
                    isMessagingDisabled: isMessagingDisabled,
                    isAssessmentEnabled: isAssessmentEnabled || isStudyCenterEnabled,
                    isPeopleEnabled: isPeopleEnabled
                };

                return sidebarOptions;
            },

            OVERRIDE, VOID, function onStop_() {
                this.apiHost_.onStop();
            },

            OVERRIDE, VOID, function onError_(error) {
                if ((!(error instanceof ria.mvc.UncaughtException) || _DEBUG) && console && error)
                    Raygun ? Raygun.send(Raygun.fetchRaygunError(error.toString())) : console.error(error.toString());

                if (error instanceof ria.mvc.MvcException) {
                    _DEBUG && console.error(error.toString());
                    var state = this.getContext().getState();
                    state.setController('error');
                    state.setAction('error404');
                    state.setParams([error.toString()]);
                    state.setPublic(false);
                    this.getContext().stateUpdated();

                    window.appInsights && window.appInsights.ChalkableTrackException(error.toString());
                }
            }

        ]);
});
