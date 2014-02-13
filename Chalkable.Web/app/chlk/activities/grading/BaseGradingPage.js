REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.grading.GradingClassSummaryTpl');
REQUIRE('chlk.templates.grading.GradingClassSummaryItemTpl');
REQUIRE('chlk.templates.grading.AnnouncementForGradingPopup');
REQUIRE('chlk.templates.grading.ItemGradingStatTpl');
REQUIRE('chlk.templates.announcement.ShortAnnouncementTpl');
REQUIRE('chlk.templates.grading.GradingClassSummaryPartTpl');
REQUIRE('chlk.models.announcement.Announcement');
REQUIRE('chlk.activities.common.InfoByMpPage');

NAMESPACE('chlk.activities.grading', function () {

    /** @class chlk.activities.grading.BaseGradingPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingClassSummaryTpl)],
        'BaseGradingPage', EXTENDS(chlk.activities.common.InfoByMpPage), [

            ArrayOf(chlk.models.grading.GradingClassSummaryItems), 'items',

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                this.setItems(this.getGradingItems_(model));
            },

            function getGradingItems_(model){
                return model.getSummaryPart().getItems();
            },

            Object, 'currentMenu',

            Number, 'currentIndex',

            OVERRIDE, VOID, function onRefresh_(model){
                BASE(model);
                var that = this;
                jQuery(this.dom.find('.ann-type-container').valueOf()).menuAim({
                    rowSelector: ".ann-button:not(.plus-ann)",
                    activate: function(row){
                        var node = new ria.dom.Dom(row);
                        node.addClass('popup-container');
                        var model = that.getAnnouncementInfo(node);
                        that.setCurrentMenu(this);
                        setTimeout(function(){
                            node.parent().find('.show-popup').trigger('click');
                        }, 1);
                        that.onPartialRender_(model, 'for-popup');
                        that.onPartialRefresh_(model, 'for-popup');
                    },
                    deactivate: function(){
                        this.clearRow();
                        new ria.dom.Dom('.ann-grade-pop-up').remove();
                    }
                });
                new ria.dom.Dom(document).on('mouseover.popup', function(node, event){
                    var target = new ria.dom.Dom(event.target);
                    var currentIndex = this.getCurrentIndex();
                    if(currentIndex
                        && !target.isOrInside('.ann-grade-pop-up')
                        && !target.isOrInside('.ann-type-container[data-index="' + currentIndex + '"]'))
                            this.getCurrentMenu() && this.getCurrentMenu().deactivate();
                }.bind(this));
                new ria.dom.Dom(document).on('click.popup', '.ann-grade-pop-up .grey-button', function(node, event){
                    var annId = node.getData('announcementid');
                    var button = this.dom.find('.ann-button[annid=' + annId + ']');
                    if(node.hasClass('dropped')){
                        button.removeClass('dropped');
                        node.addClass('x-hidden');
                        node.parent().find('.grey-button:not(.dropped)')
                            .removeClass('x-hidden')
                            .addClass('disabled');
                        this.setDropped(button, false);
                    }else{
                        button.addClass('dropped');
                        node.addClass('x-hidden');
                        node.parent().find('.grey-button.dropped')
                            .removeClass('x-hidden')
                            .addClass('disabled');
                        this.setDropped(button, true);
                    }
                }.bind(this));
            },

            OVERRIDE, VOID, function onStop_(){
                BASE();
                new ria.dom.Dom(document).off('mouseover.popup');
                new ria.dom.Dom(document).off('click.popup', '.ann-grade-pop-up .grey-button');
                new ria.dom.Dom('.ann-grade-pop-up').remove();
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.grading.AnnouncementForGradingPopup, 'for-popup')],
            VOID, function showPopup(tpl, model, msg_) {
                var target = this.dom.find('.popup-container')
                    .removeClass('popup-container');
                tpl.renderTo(new ria.dom.Dom('body'));
                var popUp = new ria.dom.Dom('.ann-grade-pop-up');
                var left = target.offset().left + target.width() + 10;
                popUp.setCss('left', left);
                popUp.setCss('top', target.offset().top - 17);
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.announcement.ShortAnnouncementTpl, chlk.activities.lib.DontShowLoader())],
            VOID, function removeDisabled(tpl, model, msg_) {
                new ria.dom.Dom('.grey-button.disabled').removeClass('disabled');
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.grading.ItemGradingStatTpl)],
            VOID, function showChart(tpl, model, msg_) {
                var popUp = new ria.dom.Dom('.announcement-popup-' + model.getAnnouncementId().valueOf());
                if(popUp.exists()){
                    tpl.renderTo(popUp.find('.chart-container'));
                }
            },

            [[ria.dom.Dom]],
            chlk.models.announcement.ShortAnnouncementViewData, function getAnnouncementInfo(node){
                var annIndex = node.parent('.announcements-type-item').getData('index');
                var typeIndex = node.parent('.ann-type-container').getData('index');
                this.setCurrentIndex(typeIndex);
                var mpIndex = node.parent('.marking-period-container').getData('index');
                var announcement = this.getItems()[mpIndex].getByAnnouncementTypes()[typeIndex].getAnnouncements()[annIndex];
                return announcement || null;
            },

            [[ria.dom.Dom, Boolean]],
            function setDropped(node, dropped){
                var annIndex = node.parent('.announcements-type-item').getData('index');
                var typeIndex = node.parent('.ann-type-container').getData('index');
                var mpIndex = node.parent('.marking-period-container').getData('index');
                var items = this.getItems();
                var announcement = items[mpIndex].getByAnnouncementTypes()[typeIndex].getAnnouncements()[annIndex];
                if(announcement){
                    announcement.setDropped(dropped);
                    this.setItems(items);
                }
            }
        ]);
});