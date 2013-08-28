REQUIRE('ria.templates.CompiledTemplate');

NAMESPACE('chlk.templates', function () {
    "use strict";

    ASSET('~/assets/jade/render-with.jade')();

    /** @class app.templates.JadeTemplate */
    CLASS(
        'JadeTemplate', EXTENDS(ria.templates.CompiledTemplate), [
            Function, 'block',
            [[Object, Number]],
            String, function getPictureURL(id, size_){
                var url = window.azurePictureUrl + id.valueOf();
                return size_ ? (url + '-' + size_ + 'x' + size_): url;
            }
        ])
});