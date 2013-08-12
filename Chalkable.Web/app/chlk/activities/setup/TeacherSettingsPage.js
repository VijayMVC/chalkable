REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.setup.TeacherSettings');
REQUIRE('chlk.templates.grading.Final');

NAMESPACE('chlk.activities.setup', function () {

    /** @class chlk.activities.setup.TeacherSettingsPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.PartialUpdateRule(chlk.templates.grading.Final, '', '.ann-types-container' , ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.TemplateBind(chlk.templates.setup.TeacherSettings)],
        'TeacherSettingsPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('click', '.next-class')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function nextButtonClick(el, event){
                var finalGradeAnnouncementTypeIds = [],percents = [], dropLowest = [],gradingStyleByType=[];
                new ria.dom.Dom('#new-items-data').find('.announcement-type-item').forEach(function(node){
                    var index = node.getAttr('index');
                    var weightNode = node.find('.announcement-type-weight');
                    finalGradeAnnouncementTypeIds[index] = weightNode.getAttr('typeId');
                    percents[index] = parseInt(weightNode.getValue() ,10);
                    dropLowest[index] = node.find('[type=checkbox]').checked();
                    gradingStyleByType[index] = node.find('select').getValue();
                });
                this.dom.find('input[name=participation]').setValue(parseInt(this.dom.find('#participation-value').getValue(), 10));
                this.dom.find('input[name=attendance]').setValue(parseInt(this.dom.find('#attendance-value').getValue(), 10));
                this.dom.find('input[name=discipline]').setValue(parseInt(this.dom.find('#discipline-value').getValue(), 10));
                this.dom.find('input[name=finalGradeAnnouncementTypeIds]').setValue(finalGradeAnnouncementTypeIds.join(','));
                this.dom.find('input[name=percents]').setValue(percents.join(','));
                this.dom.find('input[name=dropLowest]').setValue(dropLowest.join(','));
                this.dom.find('input[name=gradingStyleByType]').setValue(gradingStyleByType.join(','));
            },

            [ria.mvc.DomEventBind('click', '#tell-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function tellButtonClick(el, event){
                this.dom.find('#setup-schedule-view').addClass('opacity0');
                this.dom.find('#settings-step1').addClass('x-hidden');
                this.dom.find('#settings-step2, #settings-step3').removeClass('x-hidden');
                var node = this.dom.find('.percents-count');
                if(node.hasClass('error')){
                    setTimeout(function(){
                        node.triggerEvent('mouseover');
                    }.bind(this), 10);
                }
            },

            VOID, function checkPercents(){
                var sum = 0;
                this.dom.find('.announcement-type-item:not(.clone)').find('.announcement-type-weight').forEach(function(node){
                    sum+=parseInt(node.getValue(), 10);
                });
                var node = this.dom.find('.percents-count');
                if(sum == 100){
                    this.dom.find('.next-class, .go-back').removeClass('disabled');
                    this.dom.find('.next-class').setAttr('disabled', false);
                    node.removeClass('error')
                        .setHTML('100%')
                        .setAttr('data-tooltip', false)
                        .setData('tooltip', false);
                    setTimeout(function(){
                        jQuery(node.valueOf()[0]).trigger('mouseleave');
                    }, 10);
                }else{
                    var text = (sum > 100) ? 'Remove ' + (sum - 100) + '%' : 'Add ' + (100 - sum) + '%';
                    this.dom.find('.next-class, .go-back').addClass('disabled');
                    this.dom.find('.next-class').setAttr('disabled', true);
                    node.addClass('error')
                        .setAttr('data-tooltip', text)
                        .setData('tooltip', text)
                        .setHTML(sum + '%')
                        .triggerEvent('mouseover');
                }
            },

            [ria.mvc.DomEventBind('change', '.announcement-type-weight')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function weightChange(el, event){
                function animateDiv(active){
                    var height = 53, allHeight = 76;
                    var clone = item.clone();
                    clone.css('position', 'absolute')
                        .css('top', prevCount * allHeight + 3)
                        .appendTo(container)
                        .addClass('clone')
                        .find('input.announcement-type').val(value);
                    var clone2 = item.clone();
                    clone2.addClass('opacity0')
                        .css('height', 0)
                        .addClass('clone')
                        .find('input.announcement-type').val(value);
                    last[0] ? last.after(clone2) : container.prepend(clone2);
                    item.addClass('opacity0');
                    clone.animate({
                        top: (activeCount - (active ? 1 : 0)) * allHeight + 3  + 'px'
                    }, duration);
                    item.animate({
                        height: 0
                    }, duration);
                    clone2.animate({
                        height: height
                    }, duration + 1, function(){
                        var select = new chlk.controls.SelectControl();
                        clone2.remove();
                        clone.remove();
                        item = item.remove();
                        if(active){
                            item.removeClass('active');
                        }
                        item.removeClass('opacity0');
                        item.height('auto')
                            .find('.chzn-container').remove();
                        last.after(item);
                        var oldSelect = item.find('select[name=grading-type]')
                            .removeClass('chzn-done');
                        select.updateSelect(oldSelect);
                    });
                }

                var parseVal = parseInt(el.getValue(), 10), form = new ria.dom.Dom('#update-ann-type-submit-form');
                form.find('[name=id]').setValue(el.getAttr('typeId'));
                form.find('[name=value]').setValue(parseVal);

                el.setValue((parseVal && parseVal > 0) ? parseVal : 0);
                var node = jQuery(el.valueOf());
                var item = node.parents('.announcement-type-item');

                if(node.parents('#new-items-data')[0]){
                    var value = el.getValue() + '%';
                    var container = node.parents('.items-container');
                    var activeCount = container.find('.announcement-type-item.active').length;
                    var prevCount = item.prevAll().length;
                    var last = container.find('.announcement-type-item.active:last');
                    var duration = 100 * (Math.abs(activeCount - prevCount) || 1);

                    if(el.getValue() > 0){
                        if(!item.hasClass('active')){
                            item.addClass('active');
                            if(prevCount != activeCount){
                                animateDiv();
                            }
                        }
                    }else{
                        if(item.hasClass('active')){
                            if(prevCount == activeCount - 1){
                                item.removeClass('active');
                            }else{
                                animateDiv(true);
                            }
                        }
                    }
                }else{
                    if(el.getValue() > 0){
                        item.addClass('active');
                    }else{
                        item.removeClass('active');
                    }
                }

                setTimeout(function(){
                    item.next().find('input.announcement-type-weight').focus();
                }, 1);

                el.setValue(el.getValue() + '%');
                this.checkPercents();
            }
        ]);
});