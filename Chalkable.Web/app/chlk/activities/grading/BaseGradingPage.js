REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.grading.GradingClassSummaryTpl');
REQUIRE('chlk.templates.grading.GradingClassSummaryItemTpl');
REQUIRE('chlk.templates.grading.AnnouncementForGradingPopup');
REQUIRE('chlk.templates.grading.ItemGradingStatTpl');
REQUIRE('chlk.templates.announcement.ShortAnnouncementTpl');
REQUIRE('chlk.templates.grading.GradingClassSummaryPartTpl');
REQUIRE('chlk.models.announcement.ClassAnnouncementViewData');
REQUIRE('chlk.activities.common.InfoByMpPage');

NAMESPACE('chlk.activities.grading', function () {

    /** @class chlk.activities.grading.BaseGradingPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingClassSummaryTpl)],
        'BaseGradingPage', EXTENDS(chlk.activities.common.InfoByMpPage), [

            Array, 'items',

            function prepareGradingModel_(model){
                return model;
            },

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                var resModel = this.prepareGradingModel_(model);
                if (this.getGradingItems_(resModel))
                    this.setItems(this.getGradingItems_(resModel));
            },

            function getGradingItems_(model){
                if(model && model.getCurrentGradingBox && model.getCurrentGradingBox())
                    return model.getCurrentGradingBox().getByAnnouncementTypes();
                return null;
            },

            Object, 'currentMenu',

            Number, 'currentIndex',

            function activateMenuAim(){
                var that = this;

                jQuery(this.dom.find('.ann-type-container').valueOf()).menuAim({
                    rowSelector: ".ann-button:not(.plus-ann)",
                    activate: function(row){
                        var target = new ria.dom.Dom(row);
                        var model = that.getAnnouncementInfo(target);
                        that.setCurrentMenu(this);
                        var tpl = new chlk.templates.grading.AnnouncementForGradingPopup();
                        tpl.assign(model);
                        jQuery('.ann-grade-pop-up').remove();
                        jQuery('body').append(tpl.render());
                        var popUp = jQuery('.ann-grade-pop-up');
                        var left = target.offset().left + target.width() + 10;
                        popUp.css('left', left);
                        popUp.css('top', target.offset().top - 17).show();

                        var container = popUp.find('.grading-chart-container');
                        jQuery.getJSON(WEB_SITE_ROOT + 'Grading/ItemGradingStat', {
                            announcementId: target.parent('.announcements-type-item').find('.ann-button').getAttr('annid')
                        }, function(res){
                            if(container[0]){
                                var tpl = new chlk.templates.grading.ItemGradingStatTpl();
                                var model = new chlk.models.common.SimpleObject(res.data);
                                tpl.assign(model);
                                container.html(tpl.render());
                                container.find('.chart-container').trigger(chlk.controls.ChartEvents.CHART_UPDATE.valueOf());
                                container.addClass('processed');
                            }
                        })
                    },
                    deactivate: function(){
                        this.clearRow && this.clearRow();
                        jQuery('.ann-grade-pop-up').remove();
                    }
                });
            },

            OVERRIDE, VOID, function onPartialRefresh_(model, msg_){
                BASE(model, msg_);
                this.activateMenuAim();
            },

            OVERRIDE, VOID, function onRefresh_(model){
                BASE(model);
                this.activateMenuAim();

                new ria.dom.Dom(document).on('mouseover.popup', function(node, event){
                    var target = new ria.dom.Dom(event.target);
                    var currentIndex = this.getCurrentIndex();
                    if(currentIndex
                        && !target.isOrInside('.ann-grade-pop-up')
                        && !target.isOrInside('.ann-type-container[data-index="' + currentIndex + '"]'))
                            this.getCurrentMenu() && this.getCurrentMenu().deactivate();
                }.bind(this));

                new ria.dom.Dom(document).on('click.popup', function(node, event){
                    var target = new ria.dom.Dom(event.target);
                    if(!target.isOrInside('.ann-grade-pop-up') && this.getCurrentMenu()){
                        this.getCurrentMenu().deactivate();
                    }
                }.bind(this));
            },

            OVERRIDE, VOID, function onStop_(){
                BASE();
                new ria.dom.Dom(document).off('mouseover.popup');
                new ria.dom.Dom(document).off('click.popup');
                new ria.dom.Dom('.ann-grade-pop-up').remove();
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.ShortAnnouncementTpl, chlk.activities.lib.DontShowLoader())],
            VOID, function removeDisabled(tpl, model, msg_) {
                new ria.dom.Dom('.grey-button.disabled').removeClass('disabled');
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.grading.ItemGradingStatTpl)],
            VOID, function showChart(tpl, model, msg_) {
                var popUp = new ria.dom.Dom('.announcement-popup-' + model.getValue().announcementid);
                if(popUp.exists() && !popUp.hasClass('processed')){
                    popUp.addClass('processed');
                    tpl.renderTo(popUp.find('.grading-chart-container'));
                }
            },

            [[ria.dom.Dom]],
            chlk.models.announcement.BaseAnnouncementViewData, function getAnnouncementInfo(node){
                var dt = this.getSchoolYearServerDate();
                var annIndex = node.parent('.announcements-type-item').getData('index');
                var typeIndex = node.parent('.ann-type-container').getData('index');
                this.setCurrentIndex(typeIndex);
                if(this.getItems()[typeIndex].getAnnouncements)
                    var announcement = this.getItems()[typeIndex].getAnnouncements()[annIndex];
                //console.info(getDate() - dt);
                return announcement || null;
            }
        ]);
});
