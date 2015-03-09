REQUIRE('chlk.controls.Base');
REQUIRE('chlk.services.AppMarketService');
REQUIRE('chlk.models.apps.AppInstallGroup');
REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.BaseCheckControl*/
    CLASS(
        'BaseCheckControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                var service = this.getContext().getService(chlk.services.AppMarketService);
                this.setAppMarketService(service);
            },

            Object ,'timeOut',
            chlk.services.AppMarketService, 'appMarketService',

            [[Object]],
            Object, function processAttrs(attributes) {
                attributes.id = attributes.id || ria.dom.Dom.GID();
                this.context.getDefaultView()
                    .onActivityRefreshed(function (activity, model) {
                        this.update(jQuery('#'+attributes.id));
                }.bind(this));
                return attributes;
            },

            function updatePrice(price){
                price = price.toString();
                if(price.indexOf('.') > -1){
                    var second = price.split('.')[1];
                    if(second.length == 2){
                        return price;
                    }else{
                        return price + '0';
                    }
                }else{
                    return price;
                }
            },

            Object, function getInstallData(){
                return {
                    appId : null,
                    departments: null,
                    classes: null,
                    roles: null,
                    gradeLevels: null,
                    currentUserId: null,
                    installForJustMe : null
                }
            },

            VOID, function onPurchaseSelect(){
                var installData = this.getInstallData();
                this.getAppMarketService()
                    .getApplicationTotalPrice(
                    installData.appId,
                    installData.departments,
                    installData.classes,
                    installData.roles,
                    installData.gradeLevels,
                    installData.currentUserId
                )
                .then(function(data){
                    var price = data.getPrice()? '$' + data.getPrice(): "Free";
                    var totalprice = data.getTotalPrice() ? '$' + data.getTotalPrice(): "Free";
                    price = this.updatePrice(price);
                    totalprice = this.updatePrice(totalprice);
                    this.addPriceInfo(installData.installForJustMe, price, totalprice, data.getTotalPersonsCount());
                }, this);
            },

            [[Boolean, String, String, Number]],
            function addPriceInfo(justMe, price, totalPrice, personCount_){
                var middle = jQuery('.paycheck .middle');
                var priceInfo = middle.find('.price-info');
                var priceInfoRecordExists = priceInfo[0] != null && priceInfo[0] != undefined;
                if (priceInfoRecordExists)
                    priceInfo.remove();

                var userInfo = "";
                if(justMe) {
                    userInfo = '<div class="users-info">' +
                        '<div class="line">' +
                        '<div>Just me</div><div></div>' +
                        '</div>' +
                        '<div class="line"><div></div><div></div></div>' +
                        '</div>';
                }
                else{
                    userInfo = '<div class="users-info">' +
                        '<div class="line">' +
                        '<div>Users</div><div>' + personCount_+ '</div>' +
                        '</div>' +
                        '<div class="line">' +
                        '<div>Price per user</div><div>' + price + '</div>' +
                        '</div>' +
                        '</div>';
                }

                middle.prepend('<div class="price-info" ' + (priceInfoRecordExists ? '' : 'style="display: none;"') + '>' +
                    userInfo +
                    '<div class="total-info">' +
                    '<div class="line">' +
                    '<div>Total</div><div>' + totalPrice + '</div>' +
                    '</div>' +
                    '</div>' +
                    '</div>');

                if (!priceInfoRecordExists){
                    middle.find('.price-info').slideDown();
                }
            },

            [[Object]],
            function update(node){
                var that = this;
                var timeOut = this.getTimeOut();
                timeOut && clearTimeout(timeOut);
                timeOut = setTimeout(function(){
                    that.onPurchaseSelect();
                }, 1000);
                this.setTimeOut(timeOut);
            }
        ])
});