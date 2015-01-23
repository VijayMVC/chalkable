REQUIRE('chlk.models.apps.AppTotalPrice');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.apps', function () {

    /** @class chlk.templates.apps.InstallAppPriceTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/apps/app-install-price.jade')],
        [ria.templates.ModelBind(chlk.models.apps.AppTotalPrice)],
        'InstallAppPriceTpl', EXTENDS(chlk.templates.ChlkTemplate), [

            [ria.templates.ModelPropertyBind],
            Number, 'price',

            [ria.templates.ModelPropertyBind],
            Number, 'totalPrice',

            [ria.templates.ModelPropertyBind],
            Number, 'totalPersonsCount',

            function formatPrice(price){
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
        ])
});