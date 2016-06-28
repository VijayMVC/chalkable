REQUIRE('chlk.templates.announcement.LessonPlanGalleryDialogTpl');

NAMESPACE('chlk.activities.announcement', function () {


    var timeout;

    /** @class chlk.activities.announcement.LessonPlanGalleryDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.announcement.LessonPlanGalleryDialogTpl)],
        'LessonPlanGalleryDialog', EXTENDS(chlk.activities.lib.TemplateDialog), [

            [[ria.dom.Dom]],
            VOID, function submitFormWithStart(node){
                var form = node.parent('form');
                form.removeClass('working');
                form.find('[name=start]').setValue(0);
                form.trigger('submit');
            },

            VOID, function clearSearch(){
                var node = this.dom.find('.lp-gallery-search');
                node.setValue('');
                this.submitFormWithStart(node);
            },

            [ria.mvc.DomEventBind('keyup', '.lp-gallery-search')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function filterKeyUp(node, event){
                var value = node.getValue() || '';
                clearTimeout(timeout);
                if(value.length > 1){
                    timeout = setTimeout(function(){
                        this.submitFormWithStart(node);
                    }.bind(this), 500);
                }else{
                    if(!value.length)
                        this.clearSearch();
                }

                if(event.which == ria.dom.Keys.ENTER.valueOf()){
                    event.preventDefault();
                }
            },

            [ria.mvc.DomEventBind('keydown', '.lp-gallery-search')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function filterKeyDown(node, event){
                if(event.which == ria.dom.Keys.ENTER.valueOf()){
                    event.preventDefault();
                }
            },

            [ria.mvc.DomEventBind('focus', '.lp-gallery-search')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function filterFocus(node, event){
                var n = node.valueOf()[0];
                n.selectionStart = n.selectionEnd = n.value.length
            },

            [ria.mvc.DomEventBind('change', '#lp-gallery-sort-type')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function sortingChanged(node, event, data_){
                this.submitFormWithStart(node);

            },

            [ria.mvc.DomEventBind('change', '#lpGalleryCategoryType')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            VOID, function categoryTypeChanged(node, event, data_){
                this.submitFormWithStart(node);

            },

            [[Object, String]],
            OVERRIDE, VOID, function onPartialRefresh_(model, msg_){
                BASE(model, msg_);
                this.dom.find('.lp-gallery-search').trigger('focus');
            }

        ]
    );
});