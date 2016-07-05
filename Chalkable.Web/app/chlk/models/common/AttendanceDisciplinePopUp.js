REQUIRE('chlk.models.Popup');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.models.common', function () {
    "use strict";

    /** @class chlk.models.common.AttendanceDisciplinePopUp*/
    CLASS(
        'AttendanceDisciplinePopUp', EXTENDS(chlk.models.Popup), [
            chlk.models.people.User, 'student',

            Boolean, 'newStudent',

            String, 'action',

            String, 'controller',

            String, 'params',

            Boolean, 'ableEdit',

            [[ria.dom.Dom, ria.dom.Dom]],
            function $(target_, container_){
                BASE(target_, container_);
            }
        ]);
});