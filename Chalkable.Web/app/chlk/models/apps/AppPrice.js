NAMESPACE('chlk.models.apps', function () {
    "use strict";

    /** @class chlk.models.apps.AppPrice*/
    CLASS(
        'AppPrice', [
            Number, 'price',
            [ria.serialize.SerializeProperty('priceperclass')],
            Number, 'pricePerClass',
            [ria.serialize.SerializeProperty('priceperschool')],
            Number, 'pricePerSchool',


            function $(price_, pricePerClass_, pricePerSchool_){
                BASE();
                if (price_)
                    this.setPrice(price_);
                if (pricePerClass_){
                    this.setPricePerClass(pricePerClass_);
                }
                if (pricePerSchool_){
                    this.setPricePerSchool(pricePerSchool_);
                }
            },


            Object, function getPostData(){
               return {
                   price: this.getPrice(),
                   priceperclass: this.getPricePerClass(),
                   priceperschool: this.getPricePerSchool()
               }
            },

            String, function formatPrice(){
                var result = this.getPrice() > 0 ? "$" + this.formatPrice_(this.getPrice()) : "Free";
                return result;
            },

            function formatPrice_(p){
                var price = p.toString();

                if(price.indexOf('.') > -1){
                    var second = price.split('.')[1];
                    if(second.length != 2){
                        price = price + '0';
                    }
                }
                return price;
            }
        ]);
});