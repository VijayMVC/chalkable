REQUIRE('chlk.templates.Popup');
REQUIRE('chlk.models.common.AttendanceDisciplinePopUp');

NAMESPACE('chlk.templates.common', function () {

    /** @class chlk.templates.common.AttendanceDisciplinePopUpTpl*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/common/InfoMsg.jade')],
        [ria.templates.ModelBind(chlk.models.common.AttendanceDisciplinePopUp)],
        'AttendanceDisciplinePopUpTpl', EXTENDS(chlk.templates.Popup), [
            [ria.templates.ModelPropertyBind],
            chlk.models.people.User, 'student',

            [ria.templates.ModelPropertyBind],
            Boolean, 'newStudent',

            [ria.templates.ModelPropertyBind],
            String, 'action',

            [ria.templates.ModelPropertyBind],
            String, 'controller',

            [ria.templates.ModelPropertyBind],
            String, 'params',

            [ria.templates.ModelPropertyBind],
            Boolean, 'ableEdit'
        ])
});