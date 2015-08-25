REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.announcement.FileCabinetTpl');

NAMESPACE('chlk.activities.announcement', function () {

    var timeout;

    /** @class chlk.activities.announcement.FileCabinetDialog*/
    CLASS(
        [ria.mvc.ActivityGroup('AttachDialog')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.FileCabinetTpl)],
        'FileCabinetDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [

            [ria.mvc.DomEventBind('mouseover mouseleave', '.file-search-img')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function imgHover(node, event){
                this.dom.find('.file-search').toggleClass('hovered');
            },

            [ria.mvc.DomEventBind('focus blur', '.file-search')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function filterFocus(node, event){
                node.toggleClass('hovered');
            },

            [[ria.dom.Dom]],
            VOID, function submitFormWithStart(node){
                var form = node.parent('form');
                form.removeClass('working');
                form.find('[name=start]').setValue(0);
                form.trigger('submit');
            },

            VOID, function clearSearch(){
                var node = this.dom.find('.file-search');
                node.setValue('');
                this.submitFormWithStart(node);
                this.dom.find('.file-search-img').removeClass('opacity0');
                this.dom.find('.file-search-close').addClass('opacity0');
            },

            [ria.mvc.DomEventBind('keyup', '.file-search')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function filterKeyUp(node, event){
                var value = node.getValue() || '';
                clearTimeout(timeout);
                if(value.length > 1){
                    timeout = setTimeout(function(){
                        this.submitFormWithStart(node);
                    }.bind(this), 500);
                    this.dom.find('.file-search-img').addClass('opacity0');
                    this.dom.find('.file-search-close').removeClass('opacity0');
                }else{
                    if(!value.length)
                        this.clearSearch();
                }
            },

            [ria.mvc.DomEventBind('change', '#file-sort-type')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function sortingChanged(node, event, data){
                this.submitFormWithStart(node);
                //this.dom.find('#file-cabinet-filter').trigger('submit');
            }
        ]);
});