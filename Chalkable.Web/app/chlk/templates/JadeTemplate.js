REQUIRE('ria.templates.CompiledTemplate');

NAMESPACE('chlk.templates', function () {
    "use strict";

    ASSET('~/assets/jade/render-with.jade')();

    /** @class chlk.templates.JadeTemplate */
    CLASS(
        'JadeTemplate', EXTENDS(ria.templates.CompiledTemplate), [
            Function, 'block',
            [[Object, Number]],
            String, function getPictureURL(id, sizeH_, sizeW_){
                if(!id)
                    return null;
                var url = window.azurePictureUrl + id.valueOf();
                if (sizeH_ && sizeW_)
                    return url + '-' + sizeH_ + 'x' + sizeW_;
                if (sizeH_)
                    return url + '-' + sizeH_ + 'x' + sizeH_;
                return url;
            },

            [[Number]],
            String, function getSerial(number){
                return window.getSerial(number);
            },

            [[String]],
            String, function getRoleController(role){
                var controller = role.toLowerCase() + 's';
                if (controller.indexOf('admin') > -1)
                    controller = 'admins';
                return controller;
            }
        ])
});