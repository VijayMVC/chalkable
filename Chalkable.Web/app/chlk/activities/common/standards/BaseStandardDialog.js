REQUIRE('chlk.activities.lib.TemplateDialog');
REQUIRE('chlk.templates.standard.AddStandardsTpl');
REQUIRE('chlk.templates.standard.ItemsListTpl');
REQUIRE('chlk.templates.standard.StandardsBreadcrumbTpl');
REQUIRE('chlk.templates.standard.SelectedStandardTpl');
REQUIRE('chlk.templates.standard.StandardsMainTableTpl');

NAMESPACE('chlk.activities.common.standards', function(){
    /**@class chlk.activities.common.standards.BaseStandardDialog*/
    CLASS(
        [ria.mvc.DomAppendTo('#chlk-dialogs')],

        'BaseStandardDialog', EXTENDS(chlk.activities.lib.TemplateDialog),[

            Boolean, 'onlyOne',

            Boolean, function isSearchEnabled(){
                return this.dom.find('.search-breadcrumb-item').exists();
            },

            OVERRIDE, VOID, function onRender_(model) {
                BASE(model);
                if(model.isOnlyOne())
                    this.setOnlyOne(model.isOnlyOne());
            },

            function cloneLastLink(node){
                var clone = ria.__API.clone(node),
                    linkCnt = this.dom.find('.last-link-cnt');

                linkCnt.empty();
                clone.appendTo(linkCnt);

                this.dom.find('.breadcrumbs-cnt-clone').setHTML(this.dom.find('.breadcrumbs-cnt-main').getHTML());
            },

            [ria.mvc.DomEventBind('click', '.action-link.item-block')],
            [[ria.dom.Dom, ria.dom.Event]],
            function itemLinkClick(node, event){
                this.cloneLastLink(node);
            },

            [ria.mvc.DomEventBind('click', '.breadcrumb-item')],
            [[ria.dom.Dom, ria.dom.Event]],
            function breadcrumbClick(node, event){
                node.find('~').removeSelf();
                this.dom.find('.breadcrumbs-cnt-clone').setHTML(this.dom.find('.breadcrumbs-cnt-main').getHTML());

                if(!this.isSearchEnabled())
                    this.cloneLastLink(node);
            },

            [ria.mvc.DomEventBind('keydown', '.search-standard')],
            [[ria.dom.Dom, ria.dom.Event]],
            function searchStandardKeydown(node, event){
                if(event.which == ria.dom.Keys.ENTER.valueOf()){
                    if(!this.dom.find('.search-standard').getValue())
                        this.clearSearch();
                    else
                        node.parent('form').trigger('submit');
                    return false;
                }


            },

            [ria.mvc.DomEventBind('keypress', '.search-standard')],
            [[ria.dom.Dom, ria.dom.Event]],
            function searchStandardKeypress(node, event){
                if(event.which == ria.dom.Keys.ENTER.valueOf())
                    return false;
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

            [ria.mvc.DomEventBind('click', '#search-glass, .search-breadcrumb-item')],
            [[ria.dom.Dom, ria.dom.Event]],
            function searchIconClick(node, event){
                if(!this.dom.find('.search-standard').getValue())
                    this.clearSearch();
                else
                    this.dom.find('form.search-standard-cnt').trigger('submit');
            },

            [ria.mvc.DomEventBind('submit', 'form.search-standard-cnt')],
            [[ria.dom.Dom, ria.dom.Event]],
            function searchFormSubmit(node, event){
                this.dom.find('.browse-link').trigger('click');
            },

            [ria.mvc.DomEventBind('click', '.clear-search')],
            [[ria.dom.Dom, ria.dom.Event]],
            function clearSearchClick(node, event){
                this.dom.find('.search-standard').setValue('');
                this.dom.find('.clear-search').addClass('x-hidden');

                this.clearSearch();
            },

            function clearSearch(){
                if(this.isSearchEnabled()){
                    var lastLink = this.dom.find('.last-link-cnt').find('.action-link');
                    if(lastLink.exists()){
                        this.dom.find('.breadcrumbs-cnt-main').setHTML(this.dom.find('.breadcrumbs-cnt-clone').getHTML());
                        lastLink.trigger('click');
                    }else{
                        this.dom.find('form.search-standard-cnt').trigger('submit');
                    }
                }
            },

            [ria.mvc.DomEventBind('click', '.top-link:not(.pressed)')],
            [[ria.dom.Dom, ria.dom.Event]],
            function topLinkClick(node, event){
                var pressed = this.dom.find('.top-link.pressed');
                pressed.removeClass('pressed');
                this.dom.find(pressed.getData('cnt')).addClass('x-hidden');
                node.addClass('pressed');
                this.dom.find(node.getData('cnt')).removeClass('x-hidden');

                if(node.hasClass('browse-link'))
                    this.dom.find('.selected-item:not(.pressed)').removeSelf()
            },

            [ria.mvc.DomEventBind('click', '.able-add-item:not(.disabled)')],
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
                    if(this.isOnlyOne())
                        ids = [currentId];
                    else
                        if(ids.indexOf(currentId) == -1)
                            ids.push(currentId);

                    var arr = [];
                    if(!this.isSearchEnabled())
                        this.dom.find('.breadcrumbs-cnt-main').find('.breadcrumb-item').forEach(function(node){
                            arr.push(node.getText());
                        });

                    var selected = new chlk.models.standard.SelectedStandard(currentId, (node.getData('name') || '').toString(), (node.getData('description') || '').toString(), arr.join(' | '));
                    var tpl = new chlk.templates.standard.SelectedStandardTpl();
                    tpl.assign(selected);
                    var dom = new ria.dom.Dom().fromHTML(tpl.render());

                    this.isOnlyOne() && this.dom.find('.selected-items-cnt').empty();

                    dom.appendTo(this.dom.find('.selected-items-cnt'));

                    this.isOnlyOne() && setTimeout(function(){
                        node.parent('form').trigger('submit');
                    }, 1);
                }

                idsNode.setValue(ids.join(','));

                node.toggleClass('pressed');

                this.setSelectedText(ids);
            },

            [ria.mvc.DomEventBind('click', '.selected-item:not(.disabled)')],
            [[ria.dom.Dom, ria.dom.Event]],
            function selectedStandardClick(node, event){
                var idsNode = this.dom.find('.standard-ids'), val = idsNode.getValue(),
                    ids = val ? val.split(',') : [],
                    currentId = node.getData('id').toString(),
                    isRemove = node.hasClass('pressed'),
                    itemsWithCurrentId = this.dom.find('.item-block[data-id=' + currentId + ']');

                if(isRemove){
                    ids = ids.filter(function(id){return id != currentId});
                    itemsWithCurrentId.removeClass('pressed');
                }
                else{
                    ids.push(currentId);
                    itemsWithCurrentId.addClass('pressed');
                }


                idsNode.setValue(ids.join(','));

                //node.removeSelf();

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
                        ids = val ? val.split(',') : [], onlyOne = this.isOnlyOne();

                    this.dom.find('.able-add-item').forEach(function(node){
                        if(ids.indexOf(node.getData('id').toString()) > -1){
                            node.addClass('pressed');
                            if(onlyOne)
                                node.addClass('disabled');
                        }

                    });
                }

                //if(msg_ == 'add-breadcrumb')
                    //this.dom.find('.breadcrumbs-cnt-clone').setHTML(this.dom.find('.breadcrumbs-cnt-main').getHTML());
            }
        ]);
});