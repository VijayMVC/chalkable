REQUIRE('chlk.activities.lib.TemplatePage');
REQUIRE('chlk.templates.profile.SchoolPersonInfoPageTpl');
REQUIRE('chlk.templates.people.Addresses');

NAMESPACE('chlk.activities.profile', function () {

    /** @class chlk.activities.profile.SchoolPersonInfoPage */
    CLASS(
        [ria.mvc.DomAppendTo('#main')],
        [chlk.activities.lib.PageClass('profile')],
        [ria.mvc.PartialUpdateRule(chlk.templates.people.Addresses, '', '.adresses', ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.PartialUpdateRule(chlk.templates.profile.SchoolPersonInfoPageTpl, '', null, ria.mvc.PartialUpdateRuleActions.Replace)],
        [ria.mvc.TemplateBind(chlk.templates.profile.SchoolPersonInfoPageTpl)],

        'SchoolPersonInfoPage', EXTENDS(chlk.activities.lib.TemplatePage), [

            function $(){
                BASE();
                this._serializer = new chlk.lib.serialize.ChlkJsonSerializer();
            },

            [ria.mvc.DomEventBind('click', '.add-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function addAddressClick(node, event){
                var addressesValue = this.getAddresses();
                addressesValue.push({
                    id: null,
                    type: 0,
                    value: ''
                });
                var addressesModel = this._serializer.deserialize({items : addressesValue}, chlk.models.people.Addresses);
                this.onPartialRender_(addressesModel);
            },

            Array, function getAddresses(){
                var addressesNodes = this.dom.find('.home-address').valueOf();
                var len = addressesNodes.length;
                var addressesValue = [];
                var node;

                for(var i = 0; i < len; i++){
                    node = new ria.dom.Dom(addressesNodes[i]);
                    addressesValue.push({
                        id: node.getData('id'),
                        value: node.getValue(),
                        type: node.getData('type')
                    });
                }
                return addressesValue;
            },

            Array, function getPhones(){
                var primaryNode = this.dom.find('.primary-phone');
                var homeNode = this.dom.find('.home-phone');
                var res = [];

                res.push({
                    value: primaryNode.getValue(),
                    type : primaryNode.getData('type'),
                    isPrimary: true
                });
                res.push({
                    value: homeNode.getValue(),
                    type : homeNode.getData('type'),
                    isPrimary: false
                });
                return res;
            },

            [ria.mvc.DomEventBind('click', '#submit-info-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function submitClick(node, event){
                var addressesNode = this.dom.find('input[name="addressesValue"]');
                var phonesNode = this.dom.find('input[name="phonesValue"]');
                addressesNode.setValue(JSON.stringify(this.getAddresses()));
                phonesNode.setValue(JSON.stringify(this.getPhones()));
            },

            [ria.mvc.DomEventBind('click', '#edit-info-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function editClick(node, event){
                this.dom.find('.view-mode')
                    .removeClass('view-mode')
                    .addClass('edit-mode');
            },

            [ria.mvc.DomEventBind('click', '#cancell-edit-info-button')],
            [[ria.dom.Dom, ria.dom.Event]],
            VOID, function cancelClick(node, event){
                this.dom.find('.edit-mode')
                    .removeClass('edit-mode')
                    .addClass('view-mode');
            }
        ]);
});