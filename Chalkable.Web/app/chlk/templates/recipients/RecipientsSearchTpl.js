REQUIRE('chlk.templates.search.SiteSearch');

NAMESPACE('chlk.templates.recipients', function () {

    /** @class chlk.templates.recipients.RecipientsSearchTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/recipients/RecipientsSearchItem.jade')],
        [ria.templates.ModelBind(chlk.models.search.SearchItem)],
        'RecipientsSearchTpl', EXTENDS(chlk.templates.search.SiteSearch), [
            Object, 'disabledValues',

            function getItemCls(){
                var res = [], isGroup = this.getSearchType() == chlk.models.search.SearchTypeEnum.GROUP,
                    o = isGroup ? "groups" : "students", disabled = this.getDisabledValues()[o], id = this.getId(),
                    getId = isGroup ? "getGroupId" : "getId", currentId;
                res.push(isGroup ? "group" : "student");
                
                disabled.forEach(function(item){
                    currentId = item.getGroupId ? item.getGroupId() : item.getId();

                    if(currentId.valueOf() == id)
                        res.push('disabled');
                });

                return res.join(" ");
            }
        ])
});