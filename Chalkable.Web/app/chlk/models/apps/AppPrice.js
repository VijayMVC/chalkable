REQUIRE('ria.serialize.SJX');
REQUIRE('ria.serialize.IDeserializable');

NAMESPACE('chlk.models.apps', function () {
    "use strict";

    var SJX = ria.serialize.SJX;

    /** @class chlk.models.apps.AppPrice*/
    CLASS(
        UNSAFE, FINAL,   'AppPrice',IMPLEMENTS(ria.serialize.IDeserializable), [
            VOID, function deserialize(raw){
                this.price = SJX.fromValue(raw.price, Number);
                this.pricePerClass = SJX.fromValue(raw.priceperclass, Number);
                this.pricePerSchool = SJX.fromValue(raw.priceperschool, Number);
            },
            Number, 'price',
            Number, 'pricePerClass',
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