REQUIRE('chlk.templates.JadeTemplate');

REQUIRE('chlk.models.people.Addresses');

NAMESPACE('chlk.templates.people', function () {

    /** @class chlk.templates.people.Addresses*/
    CLASS(
        [ria.templates.TemplateBind('~/assets/jade/activities/profile/Addresses.jade')],
        [ria.templates.ModelBind(chlk.models.people.Addresses)],
        'Addresses', EXTENDS(chlk.templates.JadeTemplate), [
            [ria.templates.ModelPropertyBind],
            ArrayOf(chlk.models.people.Address), 'items'
        ])
});