NAMESPACE('chlk.models.common', function () {
    "use strict";
    /** @class chlk.models.common.ButtonColor*/
    ENUM('ButtonColor', {
        RED: 'red',
        BLUE: 'blue',
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
            [[String]],
            function $(title_){
                if (title_)
                    this.setText(title_);
            }
        ]);
});
