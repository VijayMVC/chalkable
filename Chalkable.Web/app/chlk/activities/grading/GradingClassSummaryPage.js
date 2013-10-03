REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.grading.GradingClassSummaryTpl');
REQUIRE('chlk.templates.grading.GradingClassSummaryItemTpl');
REQUIRE('chlk.templates.grading.AnnouncementForGradingPopup');
REQUIRE('chlk.templates.grading.ItemGradingStatTpl');
REQUIRE('chlk.models.announcement.Announcement');

NAMESPACE('chlk.activities.grading', function () {

    /** @class chlk.activities.grading.GradingClassSummaryPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [ria.mvc.TemplateBind(chlk.templates.grading.GradingClassSummaryTpl)],
        'GradingClassSummaryPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            [ria.mvc.DomEventBind('click', '.collapse')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function collapseClick(node, event){
                var parent = node.parent('.marking-period-container');
                parent.toggleClass('open');
                var mpData = parent.find('.mp-data');
                mpData.setCss('height', parent.hasClass('open') ? mpData.find('.ann-types-container').height() : 0);
            },

            ArrayOf(chlk.models.grading.GradingClassSummaryItems), 'items',

            OVERRIDE, VOID, function onRender_(model){
                BASE(model);
                this.setItems(model.getItems());
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
                        that.onPartialRender_(model, '');
                        that.onPartialRefresh_(model, '');
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
                }.bind(this))
            },

            OVERRIDE, VOID, function onStop_(){
                BASE();
                new ria.dom.Dom(document).off('mouseover.popup');
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.grading.AnnouncementForGradingPopup)],
            VOID, function showPopup(tpl, model, msg_) {
                var target = this.dom.find('.popup-container')
                    .removeClass('popup-container');
                tpl.renderTo(new ria.dom.Dom('body'));
                var popUp = new ria.dom.Dom('.ann-grade-pop-up');
                var left = target.offset().left + target.width() + 10;
                popUp.setCss('left', left);
                popUp.setCss('top', target.offset().top - 17);
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.grading.ItemGradingStatTpl)],
            VOID, function showChart(tpl, model, msg_) {
                var popUp = new ria.dom.Dom('.announcement-popup-' + model.getAnnouncementId().valueOf());
                if(popUp.exists()){
                    tpl.renderTo(popUp.find('.chart-container'));
                }
            },

            [[ria.dom.Dom]],
            chlk.models.announcement.Announcement, function getAnnouncementInfo(node){
                var annIndex = node.parent('.announcements-type-item').getData('index');
                var typeIndex = node.parent('.ann-type-container').getData('index');
                this.setCurrentIndex(typeIndex);
                var mpIndex = node.parent('.marking-period-container').getData('index');
                var announcement = this.getItems()[mpIndex].getByAnnouncementTypes()[typeIndex].getAnnouncements()[annIndex];
                return announcement;
            }
        ]);
});