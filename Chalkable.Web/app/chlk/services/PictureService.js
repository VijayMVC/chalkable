REQUIRE('chlk.services.BaseService');
REQUIRE('chlk.models.id.PictureId');
REQUIRE('ria.async.Future');

NAMESPACE('chlk.services', function () {
    "use strict";

    /** @class chlk.services.PictureService */
    CLASS(
        'PictureService', EXTENDS(chlk.services.BaseService), [
            [[chlk.models.id.PictureId, Number, Number]],
            String, function getPictureUrl(id, width_, height_) {
                var prefix = this.getContext().getSession().get('azurePictureUrl');
                var suffix = "";
                if (width_){
                    suffix =  height_ ? "-" + width_ + "x" + height_ : "-" + width_ + "x" + width_;
                }
                return id.valueOf() ? prefix + id.valueOf() + suffix: '#';
            }
        ])
});