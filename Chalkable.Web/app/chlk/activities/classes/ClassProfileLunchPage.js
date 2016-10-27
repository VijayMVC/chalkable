REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.classes.ClassProfileLunchTpl');

NAMESPACE('chlk.activities.classes', function () {

    /** @class chlk.activities.classes.ClassProfileLunchPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.classes.ClassProfileLunchTpl)],
        'ClassProfileLunchPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            [ria.mvc.DomEventBind('keydown', '.meal-count-input')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function countKeyDown(node, event){
                var lrub = [ria.dom.Keys.LEFT.valueOf(), ria.dom.Keys.RIGHT.valueOf(), ria.dom.Keys.UP.valueOf(), ria.dom.Keys.DOWN.valueOf()],
                    value = node.getValue() || "";
                if(lrub.indexOf(event.keyCode) > -1){
                    var currentIndex = node.getData('index'), nextIndex = -1,
                        mealsCount = node.parent('.chlk-grid').getData('meal-count'),
                        maxIndex = this.dom.find('.meal-count-input:last').getData('index');

                    switch(event.which){
                        case ria.dom.Keys.UP.valueOf(): nextIndex = currentIndex - mealsCount;break;
                        case ria.dom.Keys.DOWN.valueOf(): nextIndex = currentIndex + mealsCount;break;
                        case ria.dom.Keys.LEFT.valueOf(): if((currentIndex % mealsCount) && (node.getSelectedText() || node.getCursorPosition() == 0)) nextIndex = currentIndex - 1;break;
                        case ria.dom.Keys.RIGHT.valueOf(): if((currentIndex % mealsCount < mealsCount - 1) && (node.getSelectedText() || node.getCursorPosition() == value.length)) nextIndex = currentIndex + 1;break;
                    }

                    if(nextIndex >= 0 && nextIndex <= maxIndex){
                        var nextNode = this.dom.find('.meal-count-input[data-index=' + nextIndex + ']');
                        nextNode.trigger('focus');
                        setTimeout(function(){
                            nextNode.select();
                        });
                    }

                    return;
                }

                if ([46, 8, 9, 27, 110, 190].indexOf(event.keyCode) !== -1 ||
                    (event.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
                    (event.keyCode >= 35 && event.keyCode <= 40)) {
                    return;
                }

                if ((event.shiftKey || (event.keyCode < 48 || event.keyCode > 57)) && (event.keyCode < 96 || event.keyCode > 105) || value.length >= 2) {
                    event.preventDefault();
                }
            }

        ]);
});