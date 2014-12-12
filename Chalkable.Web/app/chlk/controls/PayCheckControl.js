REQUIRE('chlk.controls.BaseCheckControl');

NAMESPACE('chlk.controls', function () {

    /** @class chlk.controls.PayCheckControl*/
    CLASS(
        'PayCheckControl', EXTENDS(chlk.controls.BaseCheckControl), [
            OVERRIDE, VOID, function onCreate_()  {
                BASE();
                ASSET('~/assets/jade/controls/paycheck.jade')(this);
            },

            OVERRIDE, Object, function getInstallData(){
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
                var installData = {};
                for(var i = 0; i < ids.length; ++i){
                    var selectedIds = [];
                    var groupType = ids[i].id.valueOf();
                    var checkedBoxes = jQuery('.app-market-install').find('input[install-group="' + groupType + '"]:checked:not(:disabled)') || [];
                    checkedBoxes.each(function(index, elem){
                        var elemId = jQuery(elem).attr('name').split('chk-').pop();
                        selectedIds.push(elemId);
                    });

                    installData[ids[i].id.valueOf()] = selectedIds.join(',');
                }
                var appId = new chlk.models.id.AppId(jQuery('input[name=appId]').val());
                var departments = this.getAppMarketService().getIdsList(installData[chlk.models.apps.AppInstallGroupTypeEnum.DEPARTMENT.valueOf()],
                    chlk.models.id.AppInstallGroupId);
                var roles = this.getAppMarketService().getIdsList(installData[chlk.models.apps.AppInstallGroupTypeEnum.ROLE.valueOf()],
                    chlk.models.id.AppInstallGroupId);
                var classes = this.getAppMarketService().getIdsList(installData[chlk.models.apps.AppInstallGroupTypeEnum.CLAZZ.valueOf()],
                    chlk.models.id.AppInstallGroupId);
                var gradeLevels = this.getAppMarketService().getIdsList(installData[chlk.models.apps.AppInstallGroupTypeEnum.GRADELEVEL.valueOf()],
                    chlk.models.id.AppInstallGroupId);
                var currentUserId =  new chlk.models.id.AppInstallGroupId(installData[chlk.models.apps.AppInstallGroupTypeEnum.CURRENT_USER.valueOf()] || "");
                var installForJustMe = installData[chlk.models.apps.AppInstallGroupTypeEnum.CURRENT_USER.valueOf()][0] != null;

                return {
                    appId : appId,
                    roles: roles,
                    classes: classes,
                    gradeLevels : gradeLevels,
                    currentUserId : currentUserId,
                    installForJustMe : installForJustMe,
                    departments: departments
                };
            },

            OVERRIDE, VOID, function onPurchaseSelect(){
               var checkedElem = jQuery('.labeled-checkbox input[type=checkbox]:checked:not(:disabled)')[0];
                var middle = jQuery('.paycheck .middle');
                if (!checkedElem){
                    middle.find('.price-info').slideUp(function(){
                        jQuery(this).remove();
                    });
                }else{
                    BASE();
                }
            },

            [[Object]],
            OVERRIDE, function update(node){
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