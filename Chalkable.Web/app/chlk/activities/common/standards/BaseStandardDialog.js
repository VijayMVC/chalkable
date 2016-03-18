REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.standard.AddStandardsTpl');
REQUIRE('chlk.templates.standard.ItemsListTpl');
REQUIRE('chlk.templates.standard.StandardsBreadcrumbTpl');
REQUIRE('chlk.templates.standard.SelectedStandardTpl');
REQUIRE('chlk.templates.standard.StandardsMainTableTpl');

NAMESPACE('chlk.activities.common.standards', function(){
    //ASSET('~/assets/jade/common/standards/BaseStandardDialog.jade')();
    /**@class chlk.activities.common.standards.BaseStandardDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],

        'BaseStandardDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[

            [ria.mvc.DomEventBind('keydown', '.search-standard')],
            [[ria.dom.Dom, ria.dom.Event]],
            function searchStandardKeydown(node, event){
                if(event.which == ria.dom.Keys.ENTER.valueOf()){
                    node.parent('form').trigger('submit');
                }
            },

            [ria.mvc.DomEventBind('keyup', '.search-standard')],
            [[ria.dom.Dom, ria.dom.Event]],
            function searchStandardKeyup(node, event){
                var clearIcon = this.dom.find('.clear-search');

                if(this.dom.find('.search-standard').getValue())
                    clearIcon.removeClass('x-hidden');
                else
                    clearIcon.addClass('x-hidden');

            },

            [ria.mvc.DomEventBind('click', '#search-glass')],
            [[ria.dom.Dom, ria.dom.Event]],
            function searchIconClick(node, event){
                node.parent('form').trigger('submit');
            },

            [ria.mvc.DomEventBind('click', '.clear-search')],
            [[ria.dom.Dom, ria.dom.Event]],
            function clearSearchClick(node, event){
                this.dom.find('.search-standard').setValue('');
                this.dom.find('.clear-search').addClass('x-hidden');
                if(this.dom.find('.search-breadcrumb-item').exists())
                    node.parent('form').trigger('submit');
            },

            [ria.mvc.DomEventBind('click', '.top-link:not(.pressed)')],
            [[ria.dom.Dom, ria.dom.Event]],
            function topLinkClick(node, event){
                var pressed = this.dom.find('.top-link.pressed');
                pressed.removeClass('pressed');
                this.dom.find(pressed.getData('cnt')).addClass('x-hidden');
                node.addClass('pressed');
                this.dom.find(node.getData('cnt')).removeClass('x-hidden');
            },

            [ria.mvc.DomEventBind('click', '.breadcrumb-item')],
            [[ria.dom.Dom, ria.dom.Event]],
            function breadcrumbClick(node, event){
                node.find('~').removeSelf()
            },

            [ria.mvc.DomEventBind('click', '.able-add-item')],
            [[ria.dom.Dom, ria.dom.Event]],
            function standardClick(node, event){
                var idsNode = this.dom.find('.standard-ids'), val = idsNode.getValue(),
                    ids = val ? val.split(',') : [],
                    currentId = node.getData('id').toString();

                if(node.hasClass('pressed')){
                    ids = ids.filter(function(id){return id != currentId});
                    this.dom.find('.selected-item[data-id=' + currentId + ']').removeSelf();
                }
                else{
                    if(ids.indexOf(currentId) == -1)
                        ids.push(currentId);

                    var arr = [];
                    if(!this.dom.find('.search-breadcrumb-item').exists())
                        this.dom.find('.breadcrumb-item').forEach(function(node){
                            arr.push(node.getText());
                        });

                    var selected = new chlk.models.standard.SelectedStandard(currentId, (node.getData('name') || '').toString(), (node.getData('description').toString() || ''), arr.join(' | '));
                    var tpl = new chlk.templates.standard.SelectedStandardTpl();
                    tpl.assign(selected);
                    var dom = new ria.dom.Dom().fromHTML(tpl.render());

                    dom.appendTo(this.dom.find('.selected-items-cnt'));
                }

                idsNode.setValue(ids.join(','));

                node.toggleClass('pressed');

                this.setSelectedText(ids);
            },

            [ria.mvc.DomEventBind('click', '.selected-item')],
            [[ria.dom.Dom, ria.dom.Event]],
            function selectedStandardClick(node, event){
                var idsNode = this.dom.find('.standard-ids'), val = idsNode.getValue(),
                    ids = val ? val.split(',') : [],
                    currentId = node.getData('id').toString();

                ids = ids.filter(function(id){return id != currentId});

                idsNode.setValue(ids.join(','));

                this.dom.find('.able-add-item[data-id=' + currentId + ']').removeClass('pressed');

                node.removeSelf();

                this.setSelectedText(ids);
            },

            function setSelectedText(ids){
                var startIdsText = this.dom.find('.standard-ids-on-start').getValue(),
                    startIds = startIdsText ? startIdsText.split(',') : [];

                var btn = this.dom.find('.add-standard-btn'),
                    equal = ids.length == startIds.length && !startIds.some(function(id){
                            return ids.indexOf(id) == -1
                        });

                if(equal){
                    btn.setAttr('disabled', 'disabled');
                    btn.setProp('disabled', true);
                }else{
                    btn.removeAttr('disabled');
                    btn.setProp('disabled', false);
                    btn.removeClass('disabled');
                }

                var text = 'Selected ';

                if(ids.length)
                    text = text + '(' + ids.length + ')';

                this.dom.find('.selected-link').setText(text);
            },

            [[Object, String]],
            OVERRIDE, VOID, function onPartialRefresh_(model, msg_) {
                BASE(model, msg_);
                if(msg_ == 'list-update'){
                    var idsNode = this.dom.find('.standard-ids'), val = idsNode.getValue(),
                        ids = val ? val.split(',') : [];

                    this.dom.find('.able-add-item').forEach(function(node){
                        if(ids.indexOf(node.getData('id').toString()) > -1)
                            node.addClass('pressed');
                    });
                }
            }
        ]);
});