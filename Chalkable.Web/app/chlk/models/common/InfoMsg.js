REQUIRE('chlk.models.common.Button');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.InfoMsg*/
    CLASS(
        'InfoMsg', [
            [[String, String, ArrayOf(chlk.models.common.Button), String]],
            function $(text_, header_, buttons_, clazz_){
                text_ && this.setText(text_);
                clazz_ && this.setClazz(clazz_ );
                header_ && this.setHeader(header_);
                buttons_ && this.setButtons(buttons_);
            },

            String, 'text',
            String, 'header',
            String, 'clazz',
            ArrayOf(chlk.models.common.Button), 'buttons'
        ]);
});
