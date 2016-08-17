REQUIRE('chlk.templates.ChlkTemplate');

REQUIRE('chlk.models.people.Addresses');

NAMESPACE('chlk.templates.people', function () {

    /** @class chlk.templates.people.Addresses*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/profile/Addresses.jade')],
        [ria.templates.ModelBind(chlk.models.people.Addresses)],
        'Addresses', EXTENDS(chlk.templates.ChlkTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.Address), 'items'
        ])
});