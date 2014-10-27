REQUIRE('chlk.controls.Base');
REQUIRE('chlk.services.AppMarketService');
REQUIRE('chlk.models.apps.AppInstallGroup');
REQUIRE('chlk.models.id.AppId');
REQUIRE('chlk.models.id.SchoolPersonId');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.PayCheckControl*/
    CLASS(
        'PayCheckControl', EXTENDS(chlk.controls.Base), [
            OVERRIDE, VOID, function onCreate_() {
                BASE();
                ASSET('~/assets/jade/controls/paycheck.jade')(this);

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

            function getInstallData(){
               var ids = [{
                        id: chlk.models.apps.AppInstallGroupTypeEnum.CLAZZ,
                        name: 'classes'
                    }, {
                       id:chlk.models.apps.AppInstallGroupTypeEnum.GRADELEVEL,
                       name: 'gradeLevels'
                    }, {
                        id: chlk.models.apps.AppInstallGroupTypeEnum.DEPARTMENT,
                        name: 'departments'
                    }, {
                        id: chlk.models.apps.AppInstallGroupTypeEnum.ROLE,
                        name: 'roles'
                    }, {
                        id: chlk.models.apps.AppInstallGroupTypeEnum.ALL,
                        name: 'forAll'
                    }, {
                        id: chlk.models.apps.AppInstallGroupTypeEnum.CURRENT_USER,
                        name: 'currentPerson'
                    }
                ];

                var result = {};

                for(var i = 0; i < ids.length; ++i){
                    var selectedIds = [];
                    var groupType = ids[i].id.valueOf();
                    var checkedBoxes = jQuery('.app-market-install').find('input[install-group="' + groupType + '"]:checked:not(:disabled)') || [];
                    checkedBoxes.each(function(index, elem){
                        var elemId = jQuery(elem).attr('name').split('chk-').pop();
                        selectedIds.push(elemId);
                    });

                    result[ids[i].id.valueOf()] = selectedIds.join(',');
               }
               return result;
            },

            function onPurchaseSelect(){
                var middle = jQuery('.paycheck .middle');
                var checkedElem = jQuery('.labeled-checkbox input[type=checkbox]:checked:not(:disabled)')[0];
                if (!checkedElem){
                    middle.find('.price-info').slideUp(function(){
                        jQuery(this).remove();
                    });
                }else{
                    var installData = this.getInstallData();
                    var appId = new chlk.models.id.AppId(jQuery('input[name=appId]').val());

                    var departments = this.getAppMarketService().getIdsList(installData[chlk.models.apps.AppInstallGroupTypeEnum.DEPARTMENT.valueOf()],
                        chlk.models.id.AppInstallGroupId);

                    var classes = this.getAppMarketService().getIdsList(installData[chlk.models.apps.AppInstallGroupTypeEnum.CLAZZ.valueOf()],
                        chlk.models.id.AppInstallGroupId);

                    var roles = this.getAppMarketService().getIdsList(installData[chlk.models.apps.AppInstallGroupTypeEnum.ROLE.valueOf()],
                        chlk.models.id.AppInstallGroupId);

                    var classes = this.getAppMarketService().getIdsList(installData[chlk.models.apps.AppInstallGroupTypeEnum.CLAZZ.valueOf()],
                        chlk.models.id.AppInstallGroupId);

                    var gradeLevels = this.getAppMarketService().getIdsList(installData[chlk.models.apps.AppInstallGroupTypeEnum.GRADELEVEL.valueOf()],
                        chlk.models.id.AppInstallGroupId);

                    var currentUserId =  new chlk.models.id.AppInstallGroupId(installData[chlk.models.apps.AppInstallGroupTypeEnum.CURRENT_USER.valueOf()] || "");

                    this.getAppMarketService()
                        .getApplicationTotalPrice(
                            appId,
                            departments,
                            classes,
                            roles,
                            gradeLevels,
                            currentUserId
                        )
                        .then(function(data){
                            var price = data.getPrice()? '$' + data.getPrice(): "Free";
                            var totalprice = data.getTotalPrice() ? '$' + data.getTotalPrice(): "Free";
                            price = this.updatePrice(price);
                            totalprice = this.updatePrice(totalprice);
                            var installForJustMe = installData[chlk.models.apps.AppInstallGroupTypeEnum.CURRENT_USER.valueOf()][0] != null;
                            this.addPriceInfo(installForJustMe, price, totalprice, data.getTotalPersonsCount());
                        }, this);
                }
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
                jQuery('body').on('click', '.labeled-checkbox input[type=checkbox]', function(event){
                     var timeOut = that.getTimeOut();
                    timeOut && clearTimeout(timeOut);
                    timeOut = setTimeout(function(){
                        that.onPurchaseSelect();
                    }, 1000);
                    that.setTimeOut(timeOut);
                });

            }
    ])
});