REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.people.User');
REQUIRE('chlk.models.id.SchoolPersonId');
REQUIRE('chlk.models.id.SchoolId');

NAMESPACE('chlk.templates.people', function () {

    /** @class chlk.templates.people.User*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/profile/InfoView.jade')],
        [ria.templates.ModelBind(chlk.models.people.User)],
        'User', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            Boolean, 'active',

            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.Address), 'addresses',

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
            ArrayOf(chlk.models.people.Phone), 'phones',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.Role, 'role',

            [ria.templates.ModelPropertyBind],
            String, 'salutation',

            [ria.templates.ModelPropertyBind],
            chlk.models.id.SchoolId, 'schoolId',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.Phone, 'primaryPhone',

            [ria.templates.ModelPropertyBind],
            chlk.models.people.Phone, 'homePhone',

            [ria.templates.ModelPropertyBind],
            String, 'addressesValue',

            [ria.templates.ModelPropertyBind],
            String, 'phonesValue'
        ])
});