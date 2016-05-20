REQUIRE('chlk.activities.apps.AppWrapperDialog');
REQUIRE('chlk.templates.apps.MiniQuizDialog');
REQUIRE('chlk.templates.standard.StandardAutoCompleteTpl');
REQUIRE('chlk.AppApiHost');

NAMESPACE('chlk.activities.apps', function () {

    /**
     * @class chlk.activities.apps.MiniQuizDialog
     */
    CLASS(
        [ria.mvc.ActivityGroup('MiniQuizDialog')],
        [ria.mvc.DomAppendTo('#chlk-dialogs')],
        [ria.mvc.TemplateBind(chlk.templates.apps.MiniQuizDialog)],
        [ria.mvc.PartialUpdateRule(chlk.templates.apps.MiniQuizDialog, '', null , ria.mvc.PartialUpdateRuleActions.Replace)],
        'MiniQuizDialog', EXTENDS(chlk.activities.apps.AppWrapperDialog), [
            function $() {
                BASE();

                this._addedStandards = [];
            },

            [ria.mvc.PartialUpdateRule(chlk.templates.apps.MiniQuizDialog, 'content')],
            [[Object, Object, String]],
            VOID, function updateContent(tpl, model, msg_) {
                var dom = new ria.dom.Dom(tpl.render());
                this.dom.find('.x-window-body').setHTML(dom.find('.x-window-body').getHTML());
                this.refreshIframe_();
            },

            [ria.mvc.DomEventBind('click', '.standard-link:not(.pressed)')],
            [[ria.dom.Dom, ria.dom.Event]],
            function standardClick(node, event){
                this.dom.find('.standards-tb').find('.pressed').removeClass('pressed');
                setTimeout(function(){
                    node.addClass('pressed');
                }, 1);
                node.parent().addClass('pressed');
            },

            [ria.mvc.DomEventBind('click', '.standard-remove')],
            [[ria.dom.Dom, ria.dom.Event]],
            function removeStandardClick(node, event){
                var removeId = node.getData('id');
                var idsInput = this.dom.find('.standard-ids');
                var ids = JSON.parse(idsInput.getValue());
                ids = ids.filter(function(id){return id != removeId});
                idsInput.setValue(JSON.stringify(ids));
                var currentId = this.dom.find('.standard-link.pressed').getData('id');
                if(removeId != currentId)
                    this.dom.find('.current-standard-id').setValue(currentId);
                idsInput.parent('form').trigger('submit');
            },

            [ria.mvc.DomEventBind('change', '.add-new')],
            [[ria.dom.Dom, ria.dom.Event, Object]],
            function addNew(node, event, selected_){
                var name = node.getValue();
                var id = this.dom.find('[type=hidden][name=filter]').getValue();
                if(id){
                    var standardNode = this.dom.find('.standard-link[data-id=' + id + ']');
                    if(standardNode.exists()){
                        standardNode.trigger('click');
                        node.setValue('');
                        return false;
                    }

                    var standard = new chlk.models.standard.Standard(new chlk.models.id.StandardId(id), name);
                    var idsInput = this.dom.find('.standard-ids');
                    var ids = JSON.parse(idsInput.getValue());
                    ids.unshift(parseInt(id, 10));
                    idsInput.setValue(JSON.stringify(ids));
                    idsInput.parent('form').trigger('submit');
                }
            },

            [[Object]],
            OVERRIDE, VOID, function onRefresh_(data) {
                BASE(data);
                this.refreshIframe_();
            },

            [[Object, String]],
            OVERRIDE, VOID, function onPartialRefresh_(model, msg_){
                BASE(model, msg_);
                this.refreshIframe_();
            },

            function refreshIframe_(){
                var iframeExists = this.dom.find('iframe').exists();

                if (iframeExists){
                    this.dom.find('iframe').$
                        .css({height: ria.dom.Dom('iframe').$.parent().height() - 27*2 + 'px'})
                        .load(function () {
                            this.dom.find('iframe').parent()
                                .removeClass('partial-update');
                        }.bind(this));
                }
                else
                    this.dom.find('.iframe-wrap').removeClass('partial-update');
            },
        ]);
});
