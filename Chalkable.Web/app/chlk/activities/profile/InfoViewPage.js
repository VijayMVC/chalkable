REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.profile.InfoView');
REQUIRE('chlk.templates.people.Addresses');

NAMESPACE('chlk.activities.profile', function () {

    var serializer = new ria.serialize.JsonSerializer();

    /** @class chlk.activities.profile.InfoViewPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.TemplateBind(chlk.templates.profile.InfoView)],
        'InfoViewPage', EXTENDS(chlk.activities.lib.TemplatePage), [
            [ria.mvc.DomEventBind('click', '.add-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function addAddressClick(node, event){
                var addressesNode = this.dom.find('input[name="addressesValue"]');
                var addressesValue = JSON.parse(addressesNode.getValue()) || [];
                addressesValue.push({
                    id:null,
                    type:0,
                    value: ''
                });
                addressesNode.setValue()
                var addressesModel = serializer.deserialize({items : addressesValue}, chlk.models.people.Addresses);
                this.onPartialRender_(addressesModel);
            }
        ]);
});