REQUIRE('chlk.models.people.User');
REQUIRE('chlk.templates.ChlkTemplate');

NAMESPACE('chlk.templates.setup', function () {

    /** @class chlk.templates.setup.Hello*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/setup/Hello.jade')],
        [ria.templates.ModelBind(chlk.models.people.User)],
        'Hello', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            Boolean, 'active',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.Address, 'address',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.ChlkDate, 'birthDate',

            [ria.templates.ModelPropertyBind],
            String, 'birthDateText',

            [ria.templates.ModelPropertyBind],
            String, 'displayName',

            [ria.templates.ModelPropertyBind],
            String, 'email',

            [ria.templates.ModelPropertyBind],
            String, 'firstName',

            [ria.templates.ModelPropertyBind],
            String, 'fullName',

            [ria.templates.ModelPropertyBind],
            String, 'gender',

            [ria.templates.ModelPropertyBind],
            String, 'genderFullText',

            [ria.templates.ModelPropertyBind],
            String, 'grade',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolPersonId, 'id',

            [ria.templates.ModelPropertyBind],
            String, 'lastName',

            [ria.templates.ModelPropertyBind],
            String, 'localId',

            [ria.templates.ModelPropertyBind],
            String, 'password',

            [ria.templates.ModelPropertyBind],
            String, 'pictureUrl',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.Phone), 'phones',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.Role, 'role',

            [ria.templates.ModelPropertyBind],
            String, 'salutation',

            [ria.templates.ModelPropertyBind],
            Number, 'schoolId',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.Phone, 'primaryPhone',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.Phone, 'homePhone',

            [ria.templates.ModelPropertyBind],
            String, 'addressesValue',

            [ria.templates.ModelPropertyBind],
            String, 'phonesValue',

            [ria.templates.ModelPropertyBind],
            Number, 'index',

            [ria.templates.ModelPropertyBind],
            Boolean, 'selected',

            [ria.templates.ModelPropertyBind],
            chlk.models.common.Alerts, 'alertsInfo',

            [ria.templates.ModelPropertyBind],
            Boolean, 'demoUser',

            [ria.templates.ModelPropertyBind],
            Boolean, 'withdrawn'
        ])
});