NAMESPACE('chlk.models.common', function () {
    "use strict";
    /** @class chlk.models.common.ButtonColor*/
    ENUM('ButtonColor', {
        RED: 'negative-button',
        BLUE: 'blue-button',
        GREEN: 'green',
        GRAY: 'gray'
    });

    /** @class chlk.models.common.Button*/
    CLASS(
        'Button', [
            String, 'controller',
            String, 'action',
            Array, 'params',
            chlk.models.common.ButtonColor, 'color',
            Object, 'attributes',
            String, 'text',
            String, 'value',
            String, 'clazz',
            [[String, chlk.models.common.ButtonColor]],
            function $(title_, color_){
                BASE();
                if (title_)
                    this.setText(title_);
                if (color_)
                    this.setColor(color_);
            }
        ]);
});
