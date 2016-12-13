REQUIRE('chlk.models.common.Button');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.InfoMsg*/
    CLASS(
        'InfoMsg', [
            [[String, String, ArrayOf(chlk.models.common.Button), String, Boolean, String, Object, String]],
            function $(text_, header_, buttons_, clazz_, isHtmlText_, inputType_, inputValue_, inputAttrs_, iconUrl_){
                BASE();
                text_ && this.setText(text_);
                clazz_ && this.setClazz(clazz_ );
                header_ && this.setHeader(header_);
                buttons_ && this.setButtons(buttons_);
                isHtmlText_ && this.setHtmlText(isHtmlText_);
                inputType_ && this.setInputType(inputType_);
                inputValue_ && this.setInputValue(inputValue_);
                inputAttrs_ && this.setInputAttrs(inputAttrs_);
                iconUrl_ && this.setIconUrl(iconUrl_);
            },

            Boolean, 'htmlText',
            String, 'text',
            String, 'header',
            String, 'clazz',
            ArrayOf(chlk.models.common.Button), 'buttons',
            String, 'inputType',
            String, 'inputValue',
            Object, 'inputAttrs',
            String, 'iconUrl'
        ]);
});
