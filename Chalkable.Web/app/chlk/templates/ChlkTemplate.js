REQUIRE('chlk.templates.JadeTemplate');
REQUIRE('chlk.models.people.User');

NAMESPACE('chlk.templates', function () {
    "use strict";

    /** @class chlk.templates.ChlkTemplate */
    CLASS(
        'ChlkTemplate', EXTENDS(chlk.templates.JadeTemplate), [
            [[Object, Number]],
            String, function getStudentPictureURL(id, sizeH_, sizeW_, notDepartmentSpecific_){
                if(!id)
                    return null;
                var url = this.isDemoSchool() ? window.demoAzurePictureUrl : window.azurePictureUrl;

                var districtId = this.isDemoSchool() ? window.DEMO_SCHOOL_PICTURE_DISTRICT : window.school.districtid;

                if (notDepartmentSpecific_ == null)
                    url += districtId + '_';
                url += id.valueOf();

                return this.formatPictureURL_(url, sizeH_, sizeW_);
            },

            [[String, Number, Number]],
            String, function formatPictureURL_(url, sizeH_, sizeW_)
            {
                if (sizeH_ && sizeW_)
                    return url + '-' + sizeH_ + 'x' + sizeW_;
                if (sizeH_)
                    return url + '-' + sizeH_ + 'x' + sizeH_;
                return url;
            },

            [[Object, Number]],
            String, function getPictureURL(id, sizeH_, sizeW_, notDepartmentSpecific_){
                if(!id)
                    return null;
                var url = window.azurePictureUrl + id.valueOf();
                return this.formatPictureURL_(url, sizeH_, sizeW_);
            },

            [[Object, Number]],
            String, function getAppPictureURL(id, sizeH_, sizeW_){
               return this.getPictureURL(id, sizeH_, sizeW_);
            },

            Boolean, function isDemoSchool(){
                return !!window.DEMO_SCHOOL || false;
            },

            [[Number]],
            String, function getSerial(number){
                return window.getSerial(number);
            },

            [[String]],
            String, function getRoleController(role){
                var controller = role.toLowerCase() + 's';
                if (controller.indexOf('admin') > -1)
                    controller = 'admins';
                return controller;
            },

            chlk.models.common.Role, 'userRole',

            chlk.models.people.User, 'currentUser'
        ])
});